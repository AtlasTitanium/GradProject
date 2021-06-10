HELLO! Thank you for downloading this VR Guesture Recognition package! (Version 0.1)

This package includes 2 scenes:
A GuestureTrackerTest scene, which holds everything for you to start using the package with SteamVR.
And A MouseGuestureTest scene, which allows you to test the guesture system in 2D using your mouse.

The package also includes 2 prefabs:
A ready made SteamVR_Rig, which is ready to use for steamvr, and
A NeuralNetwork_PutThisOnYourHMD prefab, which you can put on your own HMD setup and link accordingly.


SETUP:
the package includes 2 important scripts:
GuestureRecognizer and PointTracker.

You put the GuestureRecognizer on a gameobject that's a CHILD of your HMD.
In the values, you can change the amount of iterations and the amount of seconds you want to record your guestures.
There's also a setup for your hidden layers (which control the amount of hidden layers the neural network uses to convert your data) (more hidden layers = more specific results = need more teaching per record)
and there's also an input for the amount of outputs you want to have. (At the moment i reccomend using only 2 or 3, to see the difference between your outcomes.

Finally, you have the amount of learning iterations you want to do each time you teach your system, and an input for the final check line (this line is a red line that confirms your movements if you did one right.

And last is the amount of point trackers you want (i reccomend not changing this at the moment, and if things don't work, maybe try deleting one of the 2 trackers.)

NEXT:
of course is adding your point trackers to the points you want to track.
I recommend having the 2 trackers on both hands at first, and adding the point tracker script to a CHILD object of your trackers.

---------------------------------------------

HOW IT WORKS:

using SteamVR:
in the GuestureTrackerTest scene, open the inspector of the right controller, and link the 3 inputs to the ones you choose.
(the first 2 are used for the teaching of input 1 and 2, and the last one is used for testing an input)

Start the scene, and click on the first bound button, and start moving!, the line will track both points, and will show a blue line once you're finished.
now, click the second button to teach the second movement, and do the same thing.

Technically, the neural network should now have learned your movements, but giving it more inputs will specify it even further, so i recommend doing the above tasks another 3 times each.

Last but not least, is to press the 3rd input, and doing 1 of the 2 movents you gave as input. Once you're finished, you should see a red line which should be in the same position/movement as you did the input. If this is tru, then CONGRATULATIONS! the neural network has learned your movement, and can now recognize it in any shape or form that's slightly simmilar.

------------------------------------------------

AND THAT'S IT FOR NOW!
of course, the system is hard to understand at the moment, but anything requires practice in life.
I'll also try to make a tutorial video about how to use the system in the future, so you'll have a more visual explanation which'll hopefully help in using it yourself.
For now, I want to thank you for trying out my unity package, and i wish you good luck trying to figure it out!



