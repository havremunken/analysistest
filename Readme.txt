AnalysisLibrary README
======================

This is just here in case someone should happen to check out this project. I doubt anyone will, but, you know..

The library is basically a playground. There are no standards, and you'll become a worse developer if you look 
at this project. I am a failure at math and algorithms is an alien concept to me. You have been warned.

I am working on a closed source project that tracks sports leagues (mostly football (soccer) leagues). A component of 
this application tries to calculate which positions the different teams could theoretically achieve, based on the 
remaining matches. The only way to do this so far has been to brute force simulate all possible combinations of
outcomes, and calculate positions to see where each team can end up. In a football match, you have three possible outcomes
- a home win, a draw, and an away win - and this means you have 3 ^ n (where n is the number of matches remaining)
combinations to go trough. Things can quickly get out of hand as n increases...

This is useful only for answering questions like "Is team X safe from relegation?" or "Can team Y still win the league".
As such, I doubt any users will be willing to wait four billion years or so for the answer, seeing how the league will
probably be finished by then anyway. 

So this project is basically an experiment to see if I can come up with a more clever way of figuring out which team
can get to where. I have some ideas, but expressing them in code isn't easy for someone as dumb as me.

The only reason this is on GitHub is that I want to learn git. :)

Well, if anyone EVER reads this, feel free to contact me at havremunken@gmail.com or as havremunken on Twitter. If you
have ideas on how to improve the search for the perfect algorithm, I would be happy to hear from you!

Thanks for reading,

Rune
