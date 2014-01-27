                     i
                     t:t,,.
         :           j.j;,,
       ;;,,          ji.t;;
        i,,:       . jt. :,,
        t;;;          . t;,,                               Y A H O O!   U I   L I B R A R Y:
        i,,,         ,, ti;;:
        i;;,:        ;,.,Lft,.  tjji                     Y U I   C O M P R E S S O R . N E T !
        :;;;;.;;     ,,: GG.; ::,i
         i,,,jjjt.   Lt,:.G  . :..:.
         t,;;;jjii itfjt;... ..,...                                Brought to you by:
        jLG,;;;;;i:.jjfti, ,tt;i...
 tjjttttttii,...  :;titt;,tt;;  ,.                                    Pure.Krome
 ;tiiiiiiiii;... ii .:jiii;jt;.  L                                        &
  ttiiii;iii;....,j  ;,jit,fj;;   ,,..:;                              freeranger
   iiitttiit;,....f  .ijjt;;fj;,tKj;;;:;      ..
       ;iiit;;    i, .;;f;iiti,,;:;GDi,:: ,, ;
            .;    :i,fL,;i,:::::. ;::iff :i;..
           : .    :,i,fjt:;,.     ;;;,:, ;, .  ,,,,it:,
                .:;:,;:iEt;:;.    ;;;;,: ;  ..  .;;,,,,:,:::
                Lt: :,:::tDt:,,  .,,,,t. .;;,: :.,,;;;DKLjj,,
               ;GL. .,:,,.;jii.,,,,,,,;;;;;;,:. ;jt,:itifWWL;:
               fLLi .::::,::jEt,.:,,,ii;;,;;;:. .:tjfj,:,,:,,,
               GLG, :::,,,,,.;jii::,,jff:,i;,:  :; ,LEEft,,;,:
              LGG,    :,:,,,,,:;GKji.tfff:ii,;;;,,.:::.,,.:itjti;.
             jGGf    :i;,,,,;;,::ffLGi,fj:,j,,:::::;jfj,:ijtjjii;.
              LG     i;,,,,,::::: tfDGL:;:::,.:,ijjjjjjjjiiDEEK :  ,;
              i,.    ::,,,,:GWWLLL:,fKKf:.:::;jjjjjjjffjtjjt:tj. iitti;
                      ,,,,.ffLEEGG:,.tftii.:,jjjjtffttLfttGtft,; jft;;i;;
                       ,,,.jffWKL,,,,:,i;:.,;,,jtGttLfftjfjjfi. .ijGDDGLti,
                       ,::,:.:.:,,,,,,:.:;GE,fD,;jjDjfLtjff;      LL;,iLDEL;.
                      ;;:.......:::,,,;f;DDj,DDE;.tjjjjft.        tttjLfi,;jGf
                      ,;,.............;i:LL,tGDG,Dj:jf,           tttt;,tDDLt;;:
                        ,......i,,::..ii, i;,fff:tff.        ;;   ittt;ttffGG:
                        :......ttiiiii;;: ..,LfL:tff.        ;;   ;ttt;ttttG
                         :.....ttit;iti:   :iitt,ifL.        .     ;tt;jttti .
                         ......iiititttiiitjGDi,jL;L. :;;                 ;i
                          .....;iititt;itttjLGD,,DDi.  ,    
                             . ,itt;ttitttttitDt,iDDL.
                             .ttiti;ttittt;;it;D,,GGGG:
                              ttitiitttttt;ttttij,,DGDt,j
                                itiitttitt;tttttij,fDDD,;:
                                   ;ittitt;ttti;itG,DDDD,:
                                      ;;tt;ttttitttEiDG: .
                                         i;ttttitttiE.
                                            ;ititttti....
                                               ;tttt:
                                                  ;i,


   How the hell do I use this thing?
 ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Well, you've probably 'nuget installed' this package, so what next?
You have a few choices.

1. Manually compress/minify and/or bundle any Css and/or Js. This is tedious
   and personally, I wouldn't bother with this. -BUT- you might find this
   solution to your problems, so go nuts and have fun.

   Sample video explaining this: http://www.youtube.com/watch?v=LzoYUsKikx0


2. Install-Package YUICompressor.NET.Web.Optimization - take advantage of the built in compression and bundling that has come
   with ASP.NET MVC4** -but- instead of using the stock compression with ASP.NET, use this titty-sparkly
   YUICompressor.NET package instead :) This will auto-wire up this package where required. 
   That said - you'll still need to define what files you want to compress, etc.

   Sample video explaing this: http://www.youtube.com/watch?v=NSHGSbViMm8

   ** Technically, this is part of the System.Web.Optimization package .. which first came baked into
   to VS2012 with an ASP.NET MVC4 project.

3. Install-Package YUICompressor.NET.MSBuild - do the compression/minification during an MSBuild process.
   Such as during a continuous integration setup.

   Sample video explaing this: http://www.youtube.com/watch?v=sFFZ0nQog8U


4. YUICompressor .NET NAnt Task - same as #3 above, but for an NAnt continuous integration setup.


Now go forth and speed up your website and save some bandwidth.


~~~~

No Unicorns were harmed in the production of this package. "Nice package you have there!"

~~~


Q.E.D.