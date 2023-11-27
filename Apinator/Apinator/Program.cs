using System.Security.Authentication;
using Akinator.Core;
using Akinator.Core.Interfaces;
using Akinator.Core.Models.Game;
using Anthropic.SDK;
using Anthropic.SDK.Completions;
using Anthropic.SDK.Constants;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<AkiDb>();

builder.Services.AddAkinator();

// builder.Services.AddOpenTelemetry()
//     .WithMetrics(builder =>
//     {
//         // builder.AddPrometheusExporter();

//         builder.AddMeter("Microsoft.AspNetCore.Hosting",
//                          "Microsoft.AspNetCore.Server.Kestrel");
//         // builder.AddView("http.server.request.duration");
//         // builder.AddView("http.server.request.duration",
//         //     new ExplicitBucketHistogramConfiguration
//         //     {
//         //         Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05,
//         //                0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
//         //     });
//     });

var app = builder.Build();

// app.MapPrometheusScrapingEndpoint();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.Use(async (context, next) =>
// {
//     var tagsFeature = context.Features.Get<IHttpMetricsTagsFeature>();
//     if (tagsFeature != null)
//     {
//         var source = context.Request.Query["utm_medium"].ToString() switch
//         {
//             "" => "none",
//             "social" => "social",
//             "email" => "email",
//             "organic" => "organic",
//             _ => "other"
//         };
//         tagsFeature.Tags.Add(new KeyValuePair<string, object?>("mkt_medium", source));
//     }

//     await next.Invoke();
// });

app.MapGet("/ping", Ping)
.WithName("Ping")
.WithOpenApi();

app.MapGet("/anecdote/{personne}", Anecdote)
.WithName("Anecdote")
.WithOpenApi();

app.MapGet("/akinator/start", AkiStart)
.WithName("Akinator Start")
.WithOpenApi();

app.MapGet("/akinator/answers", AkiListAnswers)
.WithName("Akinator Answers")
.WithOpenApi();

app.MapGet("/akinator/response/{answer}", AkiResponse)
.WithName("Akinator Response")
.WithOpenApi();

app.Run();


static async Task<string> Anecdote(string personne, AkiDb context, IConfiguration config)
{
    var key = config["Anthropic:ClaudeApiKey"];
    if(string.IsNullOrEmpty(key))
        throw new AuthenticationException("No API KEY set");

    context.anthropic ??= new AnthropicClient(new APIAuthentication(key));
    var prompt = 
    $@"N'ajoute aucune fioriture a la réponse. Contente toi de répondre à la question aussi simplement que possible.
    {AnthropicSignals.HumanSignal} raconte moi une petite Trivia sur {personne}{AnthropicSignals.AssistantSignal}";

    var parameters = new SamplingParameters()
    {
        MaxTokensToSample = 512,
        Prompt = prompt,
        Temperature = 0.0m,
        StopSequences = new[] { AnthropicSignals.HumanSignal },
        Stream = false,
        Model = AnthropicModels.Claude_v2_1
    };
    var response = await context.anthropic.Completions.GetClaudeCompletionAsync(parameters);
    // Console.WriteLine(response.Completion);
    return response.Completion;
}

static async Task<string> Ping()
{
    return "Pong";
}


static async Task<AkiHandler> AkiStart(AkiDb context, IAkinatorClient provider)
{
    // 3. Let's start a new game!
    context.game = await provider.StartNewGame(); // Start a new game (strangely isn't it?)

    var question = context.game.GetQuestion(); // Curent question

    var r = new AkiHandler()
    {
        QuestionNumber = 1,
        Question = question
    };
    System.Console.WriteLine($"Q{r.QuestionNumber} : {question} (0.0%)");
    return r;
}

static async Task<AkiAnswersHandler> AkiListAnswers(AkiDb context)
{
    var r = new AkiAnswersHandler();
    if (context.game == null)
    {
        r.Error = "PLEASE START GAME FIRST";
        return r;
    }

    r.Answers = context.game.GetAnswers(); // Possible answers

    return r;
}

static async Task<AkiHandler> AkiResponse(int answer, AkiDb context)
{
    var r = new AkiHandler();
    if (!context.game.GetAnswers().Select(a => a.Id).Contains(answer))
    {
        r.Error = "The answer was not expected";
        return r;
    }

    await context.game.Answer(answer); // Make answer and go to the next question

    var currentStep = context.game.GetStep(); // Get the current step (question number)

    var progress = context.game.GetProgress(); // Get the current progress where 0 - akinator has no idea what you have guessed and 100 - most likely akinator knows what you have guessed. More answers you make then more progress grows.
    //Console.WriteLine(progress);

    var canGuess = context.game.CanGuess(); // If progress more than 90 than return true, otherwise false

    r.QuestionNumber = currentStep;
    r.IsGuess = canGuess;
    r.Progress = progress;

    //if (canGuess )
    //{
    //    var guessedItems = await context.game.Win(); // Return guessed items. 60-70 progress will be enough to make successful guesses.
    //    r.Question = guessedItems.First().Name;
    //    return r;
    //}
    //else
    //{
    //    var question = context.game.GetQuestion();
    //    r.Question = question;
    //    return r;
    //}

    var question = context.game.GetQuestion();
    var guessedItems = await context.game.Win(); // Return guessed items. 60-70 progress will be enough to make successful guesses.
    r.Guesses = guessedItems;

    if (canGuess)
    {
        r.Question = guessedItems.First().Name;
        System.Console.WriteLine($"R{currentStep} : C'est {guessedItems.First().Name} ({progress}%)");
        return r;
    }
    else
    {
        r.Question = question;
        System.Console.WriteLine($"Q{currentStep} : {question} ({progress}%)");
        return r;
    }
}


public class AkiHandler
{
    public int QuestionNumber { get; set; }
    public string Question { get; set; }
    public float Progress { get; set; }
    public bool IsGuess { get; set; }
    public string Error { get; set; }

    public IList<GuessedItem> Guesses { get; set; }
}

public class AkiAnswersHandler
{
    public IList<Answer> Answers { get; set; }
    public string Error { get; set; }
}

public class AkiDb : DbContext
{
    public IAkinatorGame game { get; set; }

    public AnthropicClient anthropic { get; set; }
}