set packages=(SquishIt SquishIt.CoffeeScript SquishIt.Hogan SquishIt.Less SquishIt.MsIeCoffeeScript SquishIt.MsIeHogan SquishIt.Mvc SquishIt.NSass SquishIt.Sass)

for %%p in %packages% do nuget delete %%p %1 -NoPrompt -Source https://www.nuget.org/api/v2/package