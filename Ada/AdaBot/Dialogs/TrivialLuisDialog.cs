﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Bot.Connector;
using System;

namespace AdaBot.Dialogs
{
    [Serializable]
    public class TrivialLuisDialog : LuisDialog<object>
    {
        public TrivialLuisDialog(params ILuisService[] services) : base(services)
        {

        }

        private async Task TrivialCallback(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
        }

        [LuisIntent("Insult")]
        public async Task SayHello(IDialogContext context, LuisResult result)
        {
            string message = $"Ce n'est pas gentil";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Age")]
        public async Task Age(IDialogContext context, LuisResult result)
        {
            string message = $"Je n'ai pas d'age";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Compliment")]
        public async Task Compliment(IDialogContext context, LuisResult result)
        {
            string message = $"Un grand merci :)";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Greetings")]
        public async Task CompGreetingsliment(IDialogContext context, LuisResult result)
        {
            string nameUser = context.Activity.From.Name;
            string[] firstNameUser = nameUser.Split(' ');
            string message = $"Bonjour {firstNameUser[0]}";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Feelings")]
        public async Task Feelings(IDialogContext context, LuisResult result)
        {
            string message = $"Je n'ai pas d'humeur";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Home")]
        public async Task Home(IDialogContext context, LuisResult result)
        {
            string message = $"Je n'ai pas de maison";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("InfoRequest")]
        public async Task InfoRequest(IDialogContext context, LuisResult result)
        {
            string message = $"Je suis l'assistante virtuelle du MIC";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("JokeRequest")]
        public async Task JokeRequest(IDialogContext context, LuisResult result)
        {
            string message = $"C'est un schtroumpf qui court, qui tombe et qui se fait un bleu !";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Name")]
        public async Task Name(IDialogContext context, LuisResult result)
        {
            string message = $"Je m'appelle Ada";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Phone")]
        public async Task Phone(IDialogContext context, LuisResult result)
        {
            string message = $"C'est le 0498 ... tu as vraiment cru que j'allais te donner mon numéro de téléphonne ?";
            await context.PostAsync(message);
            message = $"Je ne suis pas une fille facile, MOI ...";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Reality")]
        public async Task Reality(IDialogContext context, LuisResult result)
        {
            string message = $"Je ne suis un robot";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Sex")]
        public async Task Sex(IDialogContext context, LuisResult result)
        {
            string message = $"Je suis une fille :) <3";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Thanks")]
        public async Task Thanks(IDialogContext context, LuisResult result)
        {
            string message = $"Il n'y a pas de quoi :)";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Time")]
        public async Task Time(IDialogContext context, LuisResult result)
        {
            DateTime date = DateTime.Now;
            string message = $"Nous sommes le : " + date.ToString("dd/MM/yyyy") + " et il est : " + date.Hour +"h" + date.Minute;
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Je n'ai pas compris";
            await context.PostAsync(message);
            context.Done<object>(null);
        }
    }
}