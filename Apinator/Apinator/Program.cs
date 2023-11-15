using Akinator.Core;
using Akinator.Core.Interfaces;
using Akinator.Core.Models.Game;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<AkiDb>();

builder.Services.AddAkinator();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


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



static async Task<AkiHandler> AkiStart(AkiDb context, IAkinatorClient provider)
{
    var client = provider;

    // 3. Let's start a new game!
    context.game = await client.StartNewGame(); // Start a new game (strangely isn't it?)

    var question = context.game.GetQuestion(); // Curent question

    var r = new AkiHandler()
    {
        QuestionNumber = 1,
        Question = question
    };

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
        return r;
    }
    else
    {
        r.Question = question;
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
}