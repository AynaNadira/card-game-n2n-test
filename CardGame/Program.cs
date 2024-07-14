using System;
using System.Collections.Generic;
using System.Linq;
using CardGame.Models;
using CardGame;
using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main()
    {
        using (var context = new CardGameContext())
        {
            context.Database.EnsureCreated();

            while (true)
            {
                Console.Clear();
                ResetGame(context);
                List<Card> deck = InitializeDeck();
                deck = Shuffle(deck);  // Ensure shuffled deck is used

                Player[] players = new Player[]
                {
                    new Player { Name = "Player 1" },
                    new Player { Name = "Player 2" },
                    new Player { Name = "Player 3" },
                    new Player { Name = "Player 4" }
                };

                context.Players.AddRange(players);
                context.SaveChanges();

                DistributeCards(deck, players, context);
                DisplayHands(players, context);

                var winnerResult = EvaluateWinner(players, context);

                Console.WriteLine($"The winner is {winnerResult.Player.Name} with the highest number of {winnerResult.CardValue}s.");

                Console.WriteLine("Do you want to play again? (y/n)");
                var input = Console.ReadLine();
                if (input?.ToLower() != "y")
                {
                    break;
                }
            }
        }
    }

    static void ResetGame(CardGameContext context)
    {
        Console.WriteLine("Resetting game...");
        context.Hands.RemoveRange(context.Hands);
        context.Players.RemoveRange(context.Players);
        context.Cards.RemoveRange(context.Cards);
        context.SaveChanges();
        Console.WriteLine("Game reset complete.");
    }

    static List<Card> InitializeDeck()
    {
        Console.WriteLine("Initializing deck...");
        string[] values = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        char[] suits = { '@', '#', '^', '*' };

        List<Card> deck = new List<Card>();

        foreach (var value in values)
        {
            foreach (var suit in suits)
            {
                deck.Add(new Card { Value = value, Suit = suit });
            }
        }
        Console.WriteLine("Deck initialized.");
        return deck;
    }

    static List<Card> Shuffle(List<Card> deck)
    {
        Console.WriteLine("Shuffling deck...");
        Random rand = new Random();
        deck = deck.OrderBy(c => rand.Next()).ToList();  // Shuffle the deck
        Console.WriteLine("Deck shuffled.");
        return deck;  // Return the shuffled deck
    }

    static void DistributeCards(List<Card> deck, Player[] players, CardGameContext context)
    {
        Console.WriteLine("Distributing cards...");
        for (int i = 0; i < deck.Count; i++)
        {
            var player = players[i % 4];
            var card = deck[i];
            context.Cards.Add(card);
            context.Hands.Add(new Hand { PlayerId = player.PlayerId, Card = card });
        }
        context.SaveChanges();
        Console.WriteLine("Cards distributed.");
    }

    static void DisplayHands(Player[] players, CardGameContext context)
    {
        Console.WriteLine("Displaying hands...");
        foreach (var player in players)
        {
            var hand = context.Hands.Include(h => h.Card)
                                     .Where(h => h.PlayerId == player.PlayerId)
                                     .Select(h => h.Card)
                                     .ToList();

            Console.WriteLine($"{player.Name}'s hand: {string.Join(", ", hand.Select(card => card.Value + card.Suit))}");
        }
    }

    static (Player Player, string CardValue) EvaluateWinner(Player[] players, CardGameContext context)
    {
        Console.WriteLine("Evaluating winner...");
        Dictionary<string, int> rankValues = new Dictionary<string, int>
        {
            { "2", 2 }, { "3", 3 }, { "4", 4 }, { "5", 5 }, { "6", 6 },
            { "7", 7 }, { "8", 8 }, { "9", 9 }, { "10", 10 }, { "J", 11 },
            { "Q", 12 }, { "K", 13 }, { "A", 14 }
        };

        Dictionary<char, int> suitValues = new Dictionary<char, int>
        {
            { '@', 1 }, { '#', 2 }, { '^', 3 }, { '*', 4 }
        };

        Player winner = null;
        int highestSetCount = 0;
        string highestSetValue = null;

        foreach (var player in players)
        {
            var hand = context.Hands.Include(h => h.Card)
                                     .Where(h => h.PlayerId == player.PlayerId)
                                     .Select(h => h.Card)
                                     .ToList();

            var grouped = hand.GroupBy(card => card.Value)
                              .OrderByDescending(g => g.Count())
                              .ThenByDescending(g => rankValues[g.Key])
                              .ToList();

            var maxSet = grouped.First();
            int setCount = maxSet.Count();
            string setValue = maxSet.Key;

            if (setCount > highestSetCount)
            {
                highestSetCount = setCount;
                highestSetValue = setValue;
                winner = player;
            }
            else if (setCount == highestSetCount)
            {
                if (rankValues[setValue] > rankValues[highestSetValue])
                {
                    highestSetValue = setValue;
                    winner = player;
                }
                else if (rankValues[setValue] == rankValues[highestSetValue])
                {
                    var currentPlayerHighestSuit = maxSet.Max(card => suitValues[card.Suit.GetValueOrDefault()]);
                    var winnerHand = context.Hands.Include(h => h.Card)
                                                  .Where(h => h.PlayerId == winner.PlayerId && h.Card.Value == highestSetValue)
                                                  .Select(h => h.Card)
                                                  .ToList();
                    var winnerHighestSuit = winnerHand.Max(card => suitValues[card.Suit.GetValueOrDefault()]);

                    if (currentPlayerHighestSuit > winnerHighestSuit)
                    {
                        winner = player;
                    }
                }
            }
        }

        Console.WriteLine("Winner evaluation complete.");
        return (winner, highestSetValue);
    }
}






