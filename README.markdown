##About SquishIt

SquishIt is an asset optimization library for .net web applications.  It handles combining and minifying css and javascript assets through creation of bundles.  There are currently extensions available that allow use of .less, coffeescript, SASS/SCSS and Hogan templates through SquishIt's preprocessor pipeline.  There is also an extension that writes your combined files to [Amazon S3](https://github.com/AlexCuse/SquishIt.S3) that can serve as a template for integrating with the CDN of your choosing.

For medium trust environments there is an option to build and cache bundles in-memory so that you don't need write permission in the application's working directory.  An example of setting this up for an ASP.net MVC project can be found [here](https://github.com/jetheredge/SquishIt/wiki/Using-SquishIt-programmatically-without-the-file-system).  For a WebForms project the asset controller would just be replaced with an HTTP handler.

##Installation

SquishIt comes as a NuGet package and can be installed via `PM> Install-Package SquishIt`.

Latest stable package: [![nuget version](https://img.shields.io/nuget/v/SquishIt.svg)](https://www.nuget.org/packages/SquishIt).

You can also download precompiled binaries from the [SquishIt AppHarbor page](http://squishit.apphb.com/Download).

### Building from source
You will need to have Visual Studio and [7-Zip](http://www.7-zip.org/) installed.

1. Open Visual Studio Command Prompt
2. `cd \path\to\SquishIt\tools`
3. If necessary, add 7z.exe to your path: `set PATH=%PATH%;C:\Program Files\7-Zip\`
3. Run <code>build-package <var>version-id</var></code> (<code><var>version-id</var></code> can be anything you like.)

If all went well, you should have a package called <code>tools/SquishIt-<var>version-id</var>.zip</code>.

##Thanks

The build is generously hosted and run on the [CodeBetter TeamCity](http://codebetter.com/codebetter-ci/) infrastructure.

Latest build status: [![status](http://teamcity.codebetter.com/app/rest/builds/buildType:\(id:bt516\)/statusIcon)](http://teamcity.codebetter.com/viewType.html?buildTypeId=bt516&guest=1)

We also get great tooling support from the kind folks at [JetBrains](http://jetbrains.com)
