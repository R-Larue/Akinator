# S9-Pepper - AkinaBOT
### Alexis LOCHAMBON | Batiste LALOI | Raphael LARUE

## Vidéo démonstration du robot : 

[Vidéo YouTube](https://youtu.be/I8YR_yzOMvo)

## Explication

Le programme du robot est basé sur un fichier unique `Akinator.py` qui contient tout le comportement de notre robot

L'utilisateur peut, par la voix ou le toucher : 

- Démarrer une partie
- Jouer la partie en répondant aux questions posées par le robot
- Recommencer une partie pendant ou la fin d'une partie déjà lancée

## Démarrage du robot

Pour démarrer l'application, il faut, une fois connecté en SSH au robot via la commande `ssh nao@[PEPPER ADDRESS]`, lancer les commandes suivantes :

Récupération de l'application :
```bash
# Chemin parent de l'application
cd ~/.local/share/PackageManager/apps

# Téléchargement de l'application
git clone https://github.com/R-Larue/Akinator
```

Lancement de l'application : 
```bash
# Chemin de l'application
cd ~/.local/share/PackageManager/apps/Akinator

# Lancement de l'application
python2.7 Akinator.py
```

## Déviance éthique

A la fin d'une partie, le robot donne une anecdote sur la personnes trouvée, ce qui peut amener à le robot à donner un avis politique sur la personne, ou à diffamer untel, de part la génération d'un prompt par un LLM que nous ne contrôlons pas dans son intégralité. Une décrédibilisation de personnages politiques ou gouvernementaux peut aussi advenir.

## API

L'API est écrite en .NET 7.0 et est servie sur le port `5080`.

### Installation

Il est nécessaire d'ouvrir le port `5080` sur le réseau.

Vérifier de bien avoir .NET 7.0 d'installé

1. Cloner le repository
2. Naviguer dans Apinator/Apinator

### Utilisation

1. Dans le dossier Apinator (avec le Apinator.csproj) exécuter `dotnet watch run`
2. Enjoy


Une API SwaggerGen est auto-générée sur `localhost:5080/swagger` avec la documentation de l'API.

Plusieurs fonctions sont disponibles : 

- Ping `/ping` : Reponds pong pour vérifier la connexion.
- Anecdote `/anecdote/{personne}` : Va générer une anecdote sur la personne passée à travers l'API de Claude2 (Anthropic)
- Start `/akinator/start` : Démarre une session Akinator du début et renvoie un JSON contenant la première question.
- Response `/akinator/response/{id}` : Répond à la question récupérée précédemment et renvoie un JSON avec la prochaine question. Si Akinabot est prêt à proposer une réponse, il donnera sa réponse avec le flag isGuess a `true`.
- Answers `/akinator/answers` : Une fois une session démarée, Answers renverra une liste de réponses possibles avec leur ID sous format JSON.

Voici un exemple de réponse pour Start et Response

```json
{
  "questionNumber": 0,
  "question": "string",
  "progress": 0,
  "isGuess": true,
  "error": "string",
  "guesses": [
    {
      "name": "string"
    }
  ]
}
```

