﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AdaSDK;
using System.Net.Http;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;
using AdaSDK.Models;

namespace AdaBot.Dialogs
{
    [Serializable]
    public class AdaDialog : LuisDialog<object>
    {
        private Activity _message;
        private CreateDialog customDialog = new CreateDialog();

        public AdaDialog(params ILuisService[] services) : base(services)
        {

        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            _message = (Activity)await item;
            await base.MessageReceived(context, item);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Je n'ai pas compris :/";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
            message = $"Je suis constamment en apprentissage, je vais demander à mes créateurs de m'apprendre ta phrase ;)";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("SayHello")]
        public async Task SayHello(IDialogContext context, LuisResult result)
        {
            string nameUser = _message.From.Name;
            string[] firstNameUser = nameUser.Split(' ');
            string message = $"Bonjour {firstNameUser[0]}";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetVisitsToday")]
        public async Task GetVisitsToday(IDialogContext context, LuisResult result)
        {
            AdaClient client = new AdaClient();
            List<VisitDto> visits = await client.GetVisitsToday();

            Activity replyToConversation;

            if (visits.Count == 0)
            {
                replyToConversation = _message.CreateReply("Je n'ai encore vu personne aujourd'hui... :'(");
                replyToConversation.Recipient = _message.From;
                replyToConversation.Type = "message";
            }
            else
            {
                replyToConversation = _message.CreateReply("J'ai vu " + visits.Count + " personnes aujourd'hui! :D");
                replyToConversation.Recipient = _message.From;
                replyToConversation.Type = "message";
                replyToConversation.AttachmentLayout = "carousel";
                replyToConversation.Attachments = new List<Attachment>();

                foreach (var visit in visits)
                {
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(visit.ProfilePicture.Uri)}"));

                    //Calcul la bonne année et la bonne heure.
                    DateTime today = DateTime.Today;
                    int wrongDate = visit.PersonVisit.DateVisit.Year;
                    int goodDate = DateTime.Today.Year - wrongDate;
                    string messageDate = "";
                    string firstname;
                    DateTime visitDate = visit.PersonVisit.DateVisit;

                    //Recherche du prénom de la personne
                    if (visit.PersonVisit.FirstName == null)
                    {
                        firstname = "une personne inconnue";
                    }
                    else
                    {
                        firstname = visit.PersonVisit.FirstName;
                    }

                    messageDate = customDialog.GetVisitsMessage(firstname, visitDate);

                    HeroCard plCard = new HeroCard()
                    {
                        Title = visit.PersonVisit.FirstName,
                        Text = messageDate + "(" + Convert.ToString(visit.PersonVisit.DateVisit.AddHours(1).AddYears(goodDate)) + ")",
                        //Subtitle = 
                        Images = cardImages
                        //Buttons = cardButtons
                    };

                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                }
            }

            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetLastVisitPerson")]
        public async Task GetLastVisitPerson(IDialogContext context, LuisResult result)
        {
            string firstname = result.Entities[0].Entity;
            AdaClient client = new AdaClient();
            List<VisitDto> visits = await client.GetLastVisitPerson(firstname);

            Activity replyToConversation;

            if (visits.Count == 0)
            {
                replyToConversation = _message.CreateReply("Je n'ai pas encore rencontré " + firstname + " :/ Il faudrait nous présenter! ^^");
                replyToConversation.Recipient = _message.From;
                replyToConversation.Type = "message";
            }
            else
            {
                replyToConversation = _message.CreateReply("Voyons voir...");
                replyToConversation.Recipient = _message.From;
                replyToConversation.Type = "message";
                replyToConversation.AttachmentLayout = "carousel";
                replyToConversation.Attachments = new List<Attachment>();

                foreach (var visit in visits)
                {
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(visit.ProfilePicture.Uri)}"));

                    //Calcul la bonne année et la bonne heure.
                    DateTime today = DateTime.Today;
                    int wrongDate = visit.PersonVisit.DateVisit.Year;
                    int goodDate = DateTime.Today.Year - wrongDate;
                    string messageDate = "";
                    DateTime visitDate = visit.PersonVisit.DateVisit;

                    messageDate = customDialog.GetVisitsMessage(firstname, visitDate);

                    HeroCard plCard = new HeroCard()
                    {
                        Title = visit.PersonVisit.FirstName,
                        Text = messageDate + "(" + Convert.ToString(visit.PersonVisit.DateVisit.AddHours(1).AddYears(goodDate)) + ")",
                        //Subtitle = 
                        Images = cardImages
                        //Buttons = cardButtons
                    };

                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                }
            }

            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetStatsVisits")]
        public async Task GetLastVisGetStatsVisitsitPerson(IDialogContext context, LuisResult result)
        {
            AdaClient client = new AdaClient();
            List<VisitDto> allvisits = await client.GetVisitsToday();
            int nbVisits = allvisits.Count();
            List<VisitDto> visitsReturn = new List<VisitDto>();
            List<VisitDto> tmp = allvisits.ToList();

            int age = -1;
            int age2 = -1;
            int agePerson;

            int nbEntities = result.Entities.Count();
            for (int i=0 ; i< nbEntities ; i++)
            {
                //Get actual number of visits return
                if (visitsReturn.Count() != 0)
                {
                    nbVisits = visitsReturn.Count();
                    tmp = new List<VisitDto>(visitsReturn);
                }

                if (result.Entities[i].Type == "Gender")
                {
                    string value = result.Entities[i].Entity;
                    GenderValues gender = GenderValues.Male;
                    if ( value == "femme" || value == "femmes" || value == "fille" || value == "filles")
                    {
                        gender = GenderValues.Female;
                    }
                    
                    for (int y=0; y< nbVisits; y++)
                    {
                        if (allvisits[y].PersonVisit.Gender == gender)
                        {
                            visitsReturn.Add(allvisits[i]);
                        }
                    }
                }
                else if (result.Entities[i].Type == "Emotion")
                {
                    
                }
                else if (result.Entities[i].Type == "builtin.age")
                {
                    if (age != -1)
                    {
                        age2 = Convert.ToInt32(result.Entities[i].Entity);
                    }
                    if (age == -1)
                    {
                        age = Convert.ToInt32(result.Entities[i].Entity);
                    }
                    if (i == nbEntities - 1)
                    {
                        visitsReturn.Clear();

                        if (age2 < age)
                        {
                            int ageTmp = age;
                            age = age2;
                            age2 = ageTmp;
                        }
                        if (age2 != -1)
                        {
                            //Interval of ages
                            for (int y = 0; y < nbVisits ; y++)
                            {
                                agePerson = DateTime.Today.Year - tmp[y].PersonVisit.Age;
                                if (agePerson >= age && agePerson <= age2)
                                {
                                    visitsReturn.Add(tmp[y]);
                                }
                            }
                        }
                        else
                        {
                            //Juste one age
                            for (int y = 0; y < nbVisits ; y++)
                            {
                                agePerson = DateTime.Today.Year - tmp[y].PersonVisit.Age;
                                if (agePerson == age)
                                {
                                    visitsReturn.Add(tmp[y]);
                                }
                            }
                        }
                    }
                }
            }

            //Test results
            int nbReturn = visitsReturn.Count();
        }
    }
}