# Prise-Bot
Prise-Bot is a Discord bot written in .NET 5 that allows you to get notified for a new event. It also shows a quick message for you to enjoy the event.

## Important
This bot is by no means finished, all database request are still executed on my own firebase account instead of a real RESTfull API. I post a example database structure down at **usage**though.

## Usage
At first you should setup a firestore database and use this model.
````
    /spruche/~/{Index: yourIndex, Header: "Your message header", Value: "The message"}
    /videos/~/{Index: yourIndex, Url: "Your Youtube Url"}
    /intern/spruche/{Length: "Length of the sprueche collection"}
    /intern/videos/{Length: "Length of the videos collection"}
````
    
Click publish on the PriseBot in Visual Studio. Add the Bot token in a token.txt and the firebase Json File in a firebase-key.json file int the root output. Start a Lavalink server with deafult settings on the same domain.

After you invited the bot, just send a period to let the bot claim that channel.

## Commands
````
    .prise start // Starts the bots main loop
    .pirse stop // Stops the bots main loop
````
