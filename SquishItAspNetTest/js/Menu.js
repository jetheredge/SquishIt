var menuItems = [
              { "text": "Bing", "uri": "http://www.bing.com" },
              { "text": "Google", "uri": "http://www.google.com" },
              { "text": "Ask", "uri": "http://www.ask.com" },
              { "text": "Yahoo!", "uri": "http://www.yahoo.com" },
              { "text": "Dog Pile", "uri": "http://www.dogpile.com" },
              { "text": "Local", "uri": "http://www.local.com" },
              { "text": "Stack Overflow", "uri": "http://stackoverflow.com/" }
           ];
document.writeln('<ul>');
for (var i = 0; i < menuItems.length; i++) {
    document.writeln('<li><a href="' + menuItems[i].uri + '">' + menuItems[i].text + '</a></li>');
}
document.writeln('</ul>');