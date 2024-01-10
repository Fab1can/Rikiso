const PORT = 1337
const WebSocket = require('ws');

const borders = {
	"Albareto":[],
"Amendola Est":[],
"Amendola Ovest":["Cognento"],
"Baggiovara":["Cittanova","Cognento","Saliceta"],
"Bruciata":["Cittanova","Cognento"],
"Cittanova":["Cognento","Baggiovara","Marzaglia","Bruciata","Madonnina","Tre Olmi"],
"Cognento":["Cittanova","Baggiovara","Villaggio Zeta","Saliceta","Amendola Ovest","Bruciata"],
"Collegarola":[],
"Crocetta":[],
"Fossalta":[],
"Ganaceto":[],
"La Rotonda":[],
"Lesignana":[],
"Madonnina":["Cittanova"],
"Marzaglia":["Cittanova"],
"Modena Est":[],
"Navicello":[],
"Paganine":[],
"Ponte Alto":[],
"Portile":[],
"S Damaso":[],
"S Donnino":[],
"S Maria Mugnano":[],
"S Martino Mugnano":[],
"S Matteo":[],
"S Pancrazio":[],
"Saliceta":["Cognento","Baggiovara"],
"Saliceto sul Panaro":[],
"Torrenova":[],
"Tre Olmi":["Cittanova"],
"Villaggio Zeta":["Cognento"],
"Villanova":[],
}

function dividiBorders() {
	let array = Object.keys(borders);
	// Mescola l'array casualmente
	array.sort(() => Math.random() - 0.5);
  
	// Calcola la metà dell'array
	const metaArray = Math.floor(array.length / 2);
  
	// Divide l'array in due parti
	const primoArray = array.slice(0, metaArray);
	const secondoArray = array.slice(metaArray);
  
	return [primoArray, secondoArray];
  }

players = []
playersInGame = []

function startGame(){
	console.log("Game started")
	for(id in players){
		players[id].send(id);
		playersInGame.push(players[id]);
	}
}

const wss = new WebSocket.Server({ port: PORT });

wss.on('connection', function connection(ws) {
	ws.on('message', function incoming(message) {
	  console.log('received: %s', message);
	});
	ws.on('close', function() {
        console.log("Player disconnected");
		players=players.filter((w)=>w!=ws);
    });

	console.log("Player connected");
	players.push(ws);
	if(players.length==2){
		startGame()	
	}
  });



console.log("Server started on "+PORT)