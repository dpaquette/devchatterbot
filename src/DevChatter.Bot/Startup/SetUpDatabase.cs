using DevChatter.Bot.Core.Commands;
using DevChatter.Bot.Core.Data;
using DevChatter.Bot.Core.Data.Model;
using DevChatter.Bot.Core.Data.Specifications;
using DevChatter.Bot.Infra.Ef;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using QuizQuestion = DevChatter.Bot.Core.Data.Model.QuizQuestion;

namespace DevChatter.Bot.Startup
{
    public static class SetUpDatabase
    {
        public static IRepository SetUpRepository(string connectionString)
        {
            DbContextOptions<AppDataContext> options = new DbContextOptionsBuilder<AppDataContext>()
                .UseSqlServer(connectionString)
                .Options;

            var appDataContext = new AppDataContext(options);

            EnsureDatabase(appDataContext);
            IRepository repository = new EfGenericRepo(appDataContext);
            EnsureInitialData(repository);

            return repository;
        }

        private static void EnsureDatabase(AppDataContext dataContext)
        {
            dataContext.Database.Migrate();
        }

        private static void EnsureInitialData(IRepository repository)
        {
            if (!repository.List<ScheduleEntity>().Any())
            {
                repository.Create(GetDevChatterSchedule());
            }

            if (!repository.List<IntervalMessage>().Any())
            {
                repository.Create(GetIntervalMessages());
            }

            if (!repository.List<SimpleCommand>().Any())
            {
                repository.Create(GetSimpleCommands());
            }

            if (!repository.List<QuoteEntity>().Any())
            {
                repository.Create(GetInitialQuotes());
            }

            if (!repository.List<HangmanWord>().Any())
            {
                repository.Create(GetInitialHangmanWords());
            }

            if (!repository.List<QuizQuestion>().Any())
            {
                repository.Create(GetInitialQuizQuestions());
            }

            CreateInitialRouletteSettingsIfNeeded(repository);

            var missingCommandWords = GetMissingCommandWords(repository);
            if (missingCommandWords.Any())
            {
                repository.Create(missingCommandWords);
            }
        }

        private static void CreateInitialRouletteSettingsIfNeeded(IRepository repository)
        {
            var commandName = "roulette";
            var settings = repository.List(CommandSettingsPolicy.ByCommandName(commandName));

            if (settings.All(x => x.Key != "win_percentage_chance"))
            {
                repository.Create(new CommandSettingsEntity
                {
                    CommandNameFull = commandName,
                    Key = "win_percentage_chance",
                    Value = "20"
                });
            }

            if (settings.All(x => x.Key != "timeout_duration_in_seconds"))
            {
                repository.Create(new CommandSettingsEntity
                {
                    CommandNameFull = commandName,
                    Key = "timeout_duration_in_seconds",
                    Value = "10"
                });
            }

            if (settings.All(x => x.Key != "protect_subscribers"))
            {
                repository.Create(new CommandSettingsEntity
                {
                    CommandNameFull = commandName,
                    Key = "protect_subscribers",
                    Value = "true"
                });
            }

            if (settings.All(x => x.Key != "coins_reward"))
            {
                repository.Create(new CommandSettingsEntity
                {
                    CommandNameFull = commandName,
                    Key = "coins_reward",
                    Value = "100"
                });
            }
        }

        private static List<QuizQuestion> GetInitialQuizQuestions()
        {
            return new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    MainQuestion = "Who is the best C# Twitch Streamer?",
                    Hint1 = "We aren't wearing hats...",
                    Hint2 = "Brendan is modest enough, wouldn't you say?",
                    CorrectAnswer = "DevChatter",
                    WrongAnswer1 = "CSharpFritz",
                    WrongAnswer2 = "Certainly not any of these other choices Kappa ",
                    WrongAnswer3 = "robbiew_yt",
                },
                new QuizQuestion
                {
                    MainQuestion = "Which of these is NOT valid in C#?",
                    Hint1 = "Tuples are OK by me.",
                    Hint2 = "Should I give three hints?",
                    CorrectAnswer = "int c, int d = 3;",
                    WrongAnswer1 = "int x = 4;",
                    WrongAnswer2 = "(int x, int y) = (1,2);",
                    WrongAnswer3 = "int a = 5, b = 6;",
                },
            };
        }

        private static List<ScheduleEntity> GetDevChatterSchedule()
        {
            return new List<ScheduleEntity>
            {
                new ScheduleEntity {ExampleDateTime = new DateTimeOffset(2018, 5, 7, 18, 0, 0, TimeSpan.Zero)},
                new ScheduleEntity {ExampleDateTime = new DateTimeOffset(2018, 5, 8, 18, 0, 0, TimeSpan.Zero)},
                new ScheduleEntity {ExampleDateTime = new DateTimeOffset(2018, 5, 10, 16, 0, 0, TimeSpan.Zero)},
                new ScheduleEntity {ExampleDateTime = new DateTimeOffset(2018, 5, 12, 17, 0, 0, TimeSpan.Zero)}
            };
        }

        private static List<HangmanWord> GetInitialHangmanWords()
        {
            return new List<HangmanWord>
            {
                new HangmanWord("apple"),
                new HangmanWord("banana"),
                new HangmanWord("orange"),
                new HangmanWord("mango"),
                new HangmanWord("watermellon"),
                new HangmanWord("grapes"),
                new HangmanWord("pizza"),
                new HangmanWord("pasta"),
                new HangmanWord("pepperoni"),
                new HangmanWord("cheese"),
                new HangmanWord("mushroom"),
                new HangmanWord("csharp"),
                new HangmanWord("javascript"),
                new HangmanWord("cplusplus"),
                new HangmanWord("nullreferenceexception"),
                new HangmanWord("parameter"),
                new HangmanWord("argument")
            };
        }

        private static List<IntervalMessage> GetIntervalMessages()
        {
            var automatedMessages = new List<IntervalMessage>
            {
                new IntervalMessage(15,
                    "Hello and welcome! I hope you're enjoying the stream! Feel free to follow along, make suggestions, ask questions, or contribute! And make sure you click the follow button to know when the next stream is!")
            };
            return automatedMessages;
        }

        private static List<SimpleCommand> GetSimpleCommands()
        {
            return new List<SimpleCommand>
            {
                new SimpleCommand("discord", "Hey! Checkout out our Discord here https://discord.gg/aQry9jG"),
                new SimpleCommand("github", "Check out our GitHub repositories here https://github.com/DevChatter/"),
                new SimpleCommand("emotes", "These are our current emotes: devchaHype devchaDerp devchaFail "),
                new SimpleCommand("lurk",
                    "[UserDisplayName] is just lurking here, but still thinks you're all awesome!"),
            };
        }

        private static List<QuoteEntity> GetInitialQuotes()
        {
            return new List<QuoteEntity>
            {
                new QuoteEntity
                {
                    QuoteId = 1,
                    DateAdded = new DateTime(2018, 3, 19),
                    AddedBy = "Brendoneus",
                    Author = "DevChatter",
                    Text = "Hello world!"
                },
                new QuoteEntity
                {
                    QuoteId = 2,
                    DateAdded = new DateTime(2018, 3, 19),
                    AddedBy = "Brendoneus",
                    Author = "DevChatter",
                    Text = "Welcome to DevChatter!"
                },
                new QuoteEntity
                {
                    QuoteId = 3,
                    DateAdded = new DateTime(2018, 3, 20),
                    AddedBy = "cragsify",
                    Author = "DevChatter",
                    Text = "I swear it's not rigged!"
                },
            };
        }

        private static List<CommandWordEntity> GetMissingCommandWords(IRepository repository)
        {
            var botCommandTypeAssembly = typeof(IBotCommand).Assembly;
            var conventionSuffix = "Command";

            var concreteCommands = botCommandTypeAssembly.DefinedTypes
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsSubclassOf(typeof(DataEntity)))
                .Where(x => x.FullName.EndsWith(conventionSuffix));

            var storedCommandWords = repository.List(CommandWordPolicy.OnlyPrimaries()).Select(x => x.CommandWord);

            List<CommandWordEntity> defaultCommandWords = concreteCommands
                .Select(commandType => new CommandWordEntity
                {
                    CommandWord = commandType.Name.Substring(0, commandType.Name.Length - conventionSuffix.Length),
                    FullTypeName = commandType.FullName,
                    IsPrimary = true
                })
                .Where(x => !storedCommandWords.Contains(x.CommandWord))
                .ToList();

            return defaultCommandWords;
        }
    }
}
