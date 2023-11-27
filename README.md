# S9-Pepper - AkinaBOT

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

