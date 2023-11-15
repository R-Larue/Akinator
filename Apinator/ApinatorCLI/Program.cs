// We will search for a german person server to play on.

// 1. Configure Akinator In Program.cs
using Akinator.Core;
using Akinator.Core.Interfaces;

using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddAkinator();

// 2. Then get IAkinatorClient
var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IAkinatorClient>();

// 3. Let's start a new game!
var game = await client.StartNewGame(); // Start a new game (strangely isn't it?)
var question = game.GetQuestion(); // Curent question
var answers = game.GetAnswers(); // Possible answers

await game.Answer(answers[0].Id); // Make answer and go to the next question
//await game.Back(); // You can go back to the previous question

question = game.GetQuestion();
Console.WriteLine(question);

var currentStep = game.GetStep(); // Get the current step (question number)
Console.WriteLine(currentStep);

var progress = game.GetProgress(); // Get the current progress where 0 - akinator has no idea what you have guessed and 100 - most likely akinator knows what you have guessed. More answers you make then more progress grows.
Console.WriteLine(progress);

var canGuess = game.CanGuess(); // If progress more than 90 than return true, otherwise false
Console.WriteLine(canGuess);

var guessedItems = await game.Win(); // Return guessed items. 60-70 progress will be enough to make successful guesses.

Console.WriteLine(guessedItems);
