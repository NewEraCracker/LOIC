## What I'm working on
Adding the ability to test ICMP ping flood style attacks, new to github, apologies for any version control errors, still finding my bearings


## INFO

Low Orbit Ion Cannon - An open source network stress tool, written in C#.
Based on Praetox's LOIC project at https://sourceforge.net/projects/loic/

## DISCLAIMER

This tool is released for educational purposes only, with the intent of helping server owners develop a "Hacker Defense" attitude. This tool comes without any warranty at all.

**You may not use this software for any illegal or unethical purpose; including activities which would give rise to criminal or civil liability.**

**Under no event shall the Licensor be responsible for the activities, or any misdeeds, conducted by the Licensee.**

## HOW TO RUN ON WINDOWS

GET THE BINARIES!

Requires Microsoft .NET Framework 3.5 Service Pack 1, available at:
http://www.microsoft.com/downloads/en/details.aspx?FamilyID=ab99342f-5d1a-413d-8319-81da479ab0d7&displaylang=en

## HOW TO RUN ON LINUX / MACOSX

Run debug binaries with mono.
Read the wiki at https://github.com/NewEraCracker/LOIC/wiki/ for updated instructions.

## HIVEMIND/HIDDEN MODE

HIVEMIND mode will connect your client to an IRC server so it can be controlled remotely.
Think of this as a voluntary botnet (though do beware that your client can potentially be
made to do naughty things).

Note: It does NOT allow remote administration of your machine, or anything like that; it
is literally just control of loic itself.

If you want to start up in Hivemind mode, run something like this:
```
	LOIC.exe /hivemind irc.server.address
```
It will connect to irc://irc.server.adress:6667/loic

You can also specify a port and channel:
```
	LOIC.exe /hivemind irc.server.address 1234 #secret
```
It will connect to irc://irc.server.adress:1234/secret

In order to do Hivemind Hidden mode, run something like this:
```
	LOIC.exe /hidden /hivemind irc.server.address
```
It will connect to irc://irc.server.adress:6667/loic without any visible GUI.

## CONTROLLING LOIC FROM IRC

As an OP, Admin or Owner, set the channel topic or send a message like the following:
```
	!lazor targetip=127.0.0.1 message=test_test port=80 method=tcp wait=false random=true
```

To start an attack, type:
```
	!lazor start
```

Or just append "start" to the END of the topic:
```
	!lazor targetip=127.0.0.1 message=test_test port=80 method=tcp wait=false random=true start
```

To reset loic's options back to its defaults:
```
	!lazor default
```

To stop an attack:
```
	!lazor stop
```

and be sure to remove "start" from the END of the topic, if it exists, too.

Take a look at source code for more details.
