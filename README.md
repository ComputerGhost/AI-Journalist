# AI Journalist

Automatically generate news articles when a Celebrity updates their Instagram.

The goal is to be similar to sites like <https://newsen.com> and <https://heraldcorp.com>.  When a celebrity posts on Instagram, they post the photos along with a description and a reminder of current events regarding the celebrity.  It's good for the news site, because it pulls in fans who like having a record of the posts.  It's good for the celebrities, because those additional readers see that there is activity to enjoy.  It's a win-win!

I made this for <https://leggonews.com>.  I'll maintain the code for my use.  If you want to use it on another site, you'll have to adapt it yourself.

## Design

I didn't have much time to program this, so I took a shortcut.  My code is basically just tying together a bunch of libraries and web services.  I really did barely nothing! 😂

First step is loading settings.  I think the file contents are organized well.  Most of the settings are grouped into "context" modules.  Anyways, to load the file, I use a whole two lines of code to dump it into a class.

Second step is actually pulling the updates.  Instagram's API is crap, so...  this was the most complicated part.  Instead of using their "Graph API", I went with a sort-of web scraper technique.  It was a necessary evil.  Don't look at "Instagram.cs" unless it breaks.

Third step is figuring out what each update is about.  This is done with our "Contexts" classes.  Each one adds a certain type of information about the update.

Fourth step is writing the article.  The Fluid template engine is used and attached to the contexts we figured out.  We have flexibility in the article layout and organization this way.

Fifth step is publishing!  Just send to WordPress.

## How to Setup

As mentioned, I made this for a specific website.  You might have to do a bit more than the steps below, but this should get you started...

 1) Get API credentials for: [Google Calendar](<https://console.developers.google.com/>), [Azure](https://azure.microsoft.com/en-us/services/cognitive-services/computer-vision/), [Papago](https://developers.naver.com/products/nmt/), and your WordPress installation.
 2) Rename "settings-example.json" to "settings.json".  (Note: the "settings.json" file is in the .gitignore.)
 3) Add in your API credentials, and add in the calendars you want it to pull events from.
 4) Put in your `FollowedAccounts` and `Contexts.People`.
 5) Run it and enjoy!

Oh...  what are the emoticon settings?  Yea, that goes back to the thing about it being made for my use case.  In kpop, each member typically has an associated emoticon.  On the calendars I'm pulling from, the emoticons are used as a shortcut in the title to say which members are involved in an event.

