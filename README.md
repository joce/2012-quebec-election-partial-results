Résultats préliminaires des élections générales Québec 2012
===========================================================

Résultats
---------
* [Résultats incomplets de La Presse, incluant les donnés de langage du ressensement de 2006](https://github.com/joce/2012-quebec-election-partial-results/blob/master/LaPresse_data_2012.csv)
* [Résultats préliminaires tels que trouvés sur le site du DGE](https://github.com/joce/2012-quebec-election-partial-results/blob/master/qc2012_resultats.csv)
* [Noms des candidats par circonscription et par parti](https://github.com/joce/2012-quebec-election-partial-results/blob/master/qc2012_candidats.csv)
* [Liste des identificateurs du DGE pour chaque circonscription](https://github.com/joce/2012-quebec-election-partial-results/blob/master/circonscriptions.csv)

Breve explication du pourquoi
-----------------------------
J'ai tenté en vain de trouver les résultats préliminaires des élections en format électronique. J'ai trouvé les résultats prémâchés (i.e. seulement les pourcentages des principaux partis) 
sur le site de [La Presse](http://www.lapresse.ca/actualites/elections-quebec-2012/analysez-les-resultats-du-scrutin-quebec-2012/). Et encore, l'information était cachée dans un fichier .CSV
qui était lu par un .js loadé par la page. L'avantage du data de La Presse, c'est qu'il inclus de l'information quand à la composition linguisitique de chaque circonscription.

Pour trouver les résultats préliminaires détaillés, je me suis tourné vers le [site du DGE pour les élections](http://monvote.qc.ca/fr/resultatsPreliminaires.asp). Encore une fois, le data n'est pas
facilement accessible. Les résultats de chaque cisconscription sont storés indépendemment dans autant de fichiers Javascript qu'il y a de cicronscriptions. Et non pas dans un format facilement
lisible comme JSON, YAML, XML, etc. Non. Le data est sorté dans des variables Javascript (blah!)

J'ai donc du écrire un parser qui load tous les fichiers, ne préserve que les données essentielles (toutes les données qui peuvent être calculées ont été laissées de côté) et domper le tout
dans quelques fichiers CSV.

Voilà.

AVIS DE NON-RESPONSABILITÉ
--------------------------

JE NE ME PORTE PAS GARANT DE LA VALIDITÉ DES RÉSULTATS. ILS SONT À MA CONNAISSANCE IDENTIQUES À CEUX TROUVÉS SUR LE SITE DU DGE, MAIS DES ERREURS AURAIENT PU SE GLISSER DANS MES FICHIER.
POUR AVOIR LES DONNÉES OFFICIELLES, VEUILLEZ VOUS RÉFÉRER AU SITE DU DGE.


Preliminary results for the 2012 Québec general elections
=========================================================

Results
-------
* [Incomplete La Presse results, including language data from the 2006 census](https://github.com/joce/2012-quebec-election-partial-results/blob/master/LaPresse_data_2012.csv)
* [Preliminary results as found on the DGE's website](https://github.com/joce/2012-quebec-election-partial-results/blob/master/qc2012_resultats.csv)
* [Name of all candidates, by riding and by party](https://github.com/joce/2012-quebec-election-partial-results/blob/master/qc2012_candidats.csv)
* [DGE's ridings ID list](https://github.com/joce/2012-quebec-election-partial-results/blob/master/circonscriptions.csv)

Why?
----

Long story short: I couldn't find the data in proper electronic format. I wrote a scraper to get the data from the [DGE's website](http://monvote.qc.ca/en/resultatsPreliminaires.asp). Here are the results.

DISCLAIMER
----------

I PROVIDE NO GUARANTEE AS TO THE VALIDITY OF THE RESULTS. THEY ARE TO THE BEST OF MY KNOWLEDGE IDENTICAL TO THOSE FOUND ON THE DGE'S WEBSITE BUT ERRORS COULD HAVE MADE IT IN THE DATA. FOR THE 
OFFICIAL DATA, PLEASE REFER TO THE DGE'S WEBSITE.
