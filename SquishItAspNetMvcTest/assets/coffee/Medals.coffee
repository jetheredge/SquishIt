gold = silver = rest = "unknown"

awardMedals = (first, second, others...) ->
  gold   = first
  silver = second
  rest   = others

contenders = [
  "Michael Phelps"
  "Liu Xiang"
  "Yao Ming"
  "Allyson Felix"
  "Shawn Johnson"
  "Roman Sebrle"
  "Guo Jingjing"
  "Tyson Gay"
  "Asafa Powell"
  "Usain Bolt"
]

awardMedals contenders...

$ -> 
    _goldAndSilver = $ '#goldandsilver'
    _others = $ "#others"

    _goldAndSilver.append "<li>#{gold}</li><li>#{silver}</li>"    

    for other in rest
        _others.append "<li>#{other}</li>"
