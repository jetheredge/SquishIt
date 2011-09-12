var awardMedals, contenders, gold, rest, silver;
var __slice = Array.prototype.slice;
gold = silver = rest = "unknown";
awardMedals = function() {
  var first, others, second;
  first = arguments[0], second = arguments[1], others = 3 <= arguments.length ? __slice.call(arguments, 2) : [];
  gold = first;
  silver = second;
  return rest = others;
};
contenders = ["Michael Phelps", "Liu Xiang", "Yao Ming", "Allyson Felix", "Shawn Johnson", "Roman Sebrle", "Guo Jingjing", "Tyson Gay", "Asafa Powell", "Usain Bolt"];
awardMedals.apply(null, contenders);
$(function() {
  var other, _goldAndSilver, _i, _len, _others, _results;
  _goldAndSilver = $('#goldandsilver');
  _others = $("#others");
  _goldAndSilver.append("<li>" + gold + "</li><li>" + silver + "</li>");
  _results = [];
  for (_i = 0, _len = rest.length; _i < _len; _i++) {
    other = rest[_i];
    _results.push(_others.append("<li>" + other + "</li>"));
  }
  return _results;
});