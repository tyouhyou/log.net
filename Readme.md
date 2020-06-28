# log.net

This is a logging utility for .Net projects.

*TL;NR*

Outputting logs is very easy. Include Log.cs in your project and code like this:
```
using zb;

Log.D("Hello bugs");
Log.I("For your information");
Log.W("Caution! Heaven is falling!");
Log.E("What's wrong? Heaven fell!");
```
The outputs look like this:
```
[DEBUG][2020/06/28 13:56:21][Test.cs][27][Main] - De bugs
[INFO][2020/06/28 13:56:21] - For your information
[WARN][2020/06/28 13:56:21] - Caution, the sky is falling!
[ERROR][2020/06/28 13:56:21][Test.cs][30][Main] - What's wrong? Heaven fell!
```

*Support*
- .Net Core 1.0 and above
- .Net Framework 4.5 and above, 

## Introduction
As you might notice, there has extra information at the beginning of the log line, before the message you passed. The extra information inludes log level, logging time stamp, etc. And in DEBUG and ERROR logs, code file name, code line, method name are also included.

Do not use Log class instance to output log messages. In fact, instance can not be created by using ```new Log();``` Do not bother with the boring stuffs. Just pass your messages to the public static methods in Log class:
- D(string msg) : outputing debug message
- I(string msg) : outputing info message
- W(string msg) : outputing warning message
- E(string msg) : outputing error message

## Specify log file(s)
To tell the methods to which file to output the messages, you have to choice.
1. If only one log file in the projects, ```Log.LogFile = "test/result.log";``` before any log method being called. And call logging methods with one argument, e.g. ```Log.E(msg);```
2. If multiple log files is desired, pass two arguments to the logging methods. The first one is message and the second one is log file. e.g. ```Log.W(msg, file);```

## Log levels
There have 4 log levels, the later the greater.
- DEBUG
- INFO
- WARNING
- ERROR

You can set messages at which level and above can be logged. The default log level is DEBUG in Debug mode, and INFO in Release mode.

In your product, sometimes even INFO is not desired I guess. In that case, before any logging methods called, ```Log.LogLevel = Log.Level.WARN;``` will do the trick. Now debug and info messages shut up, just WARN and ERROR will go to log file(s).

## Performance
To make outputting faster, the log file is not closed during the application life span. It will be closed automatically when the process exits. If this is not the way desired, set preprocessor constants ```LOG_CLOSE_FILE``` in the project file. This trick will make the log file to be openned for writing and closed after writing ends.

Open-and-Close each time causes bad performance. On my PC, it's 100~1000 times slower than Not-Close-File.

## Thread safe
By default, the outputting is not thread safe. To make it enabled, setting preprocessor constants of ```LOG_LOCK``` will do the trick.

*******
Enjoy, and any bug reporting and suggestion is welcome.
