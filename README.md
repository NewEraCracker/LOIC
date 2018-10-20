## INFO

Low Orbit Ion Cannon (**LOIC**) is an open source network stress tool, written in C#.
LOIC is based on Praetox's LOIC project at https://sourceforge.net/projects/loic/ .

## DISCLAIMER

LOIC is for educational purposes only, intended to help server owners develop a "hacker defense" attitude. This tool comes without any warranty.

**You may not use this software for illegal or unethical purposes. This includes activities which give rise to criminal or civil liability.**

**Under no event shall the licensor be responsible for any activities, or misdeeds, by the licensee.**

## HOW TO RUN ON WINDOWS

GET THE BINARIES!

Requires Microsoft .NET Framework 3.5 Service Pack 1, available at:
http://www.microsoft.com/downloads/en/details.aspx?FamilyID=ab99342f-5d1a-413d-8319-81da479ab0d7&displaylang=en

## HOW TO RUN ON LINUX / MACOSX

Run debug binaries with Mono or Wine.
Read the wiki at https://github.com/NewEraCracker/LOIC/wiki/_pages for updated instructions.

## HIVEMIND/HIDDEN MODE

HIVEMIND mode will connect your client to an IRC server so it can be controlled remotely.
Think of this as a voluntary botnet. Please be aware that your client can potentially be
made to do naughty things.

Note: It does NOT allow remote administration of your machine; it 
just providees control of LOIC itself.

If you want to start up in Hivemind mode, run something such as this:
```
	LOIC.exe /hivemind irc.server.address
```
which will connect to irc://irc.server.adress:6667/loic

You can also specify a port and channel:
```
	LOIC.exe /hivemind irc.server.address 1234 #secret
```
which will connect to irc://irc.server.adress:1234/secret

In order to run Hivemind Hidden mode, run something such as this:
```
	LOIC.exe /hidden /hivemind irc.server.address
```
which will connect to irc://irc.server.adress:6667/loic without any visible GUI.

## CONTROLLING LOIC FROM IRC

As an OP, Admin or Owner, set the channel topic or send a message such as the following:
```
	!lazor targetip=127.0.0.1 message=test_test port=80 method=tcp wait=false random=true
```

To start an attack, type:
```
	!lazor start
```

or append "start" to the END of the topic:
```
	!lazor targetip=127.0.0.1 message=test_test port=80 method=tcp wait=false random=true start
```

To reset LOIC's options back to their defaults:
```
	!lazor default
```

To stop an attack:
```
	!lazor stop
```

and **be sure to remove "start" from the END of the topic**, if it exists, as well.

Take a look at the source code for more details.