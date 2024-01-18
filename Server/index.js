const PORT = 1337
const WebSocket = require('ws');

const lands = {
	"Albareto": { "borders" : [] },
"Amendola Est": { "borders" : [] },
"Amendola Ovest": { "borders" : ["Cognento"] },
"Baggiovara": { "borders" : ["Cittanova","Cognento"] },
"Bruciata": { "borders" : ["Cittanova","Cognento"] },
"Cittanova": { "borders" : ["Cognento","Baggiovara","Marzaglia","Bruciata","Madonnina","Tre Olmi"] },
"Cognento": { "borders" : ["Cittanova","Baggiovara","Villaggio Zeta","Saliceta","Amendola Ovest","Bruciata"] },
"Collegarola": { "borders" : [] },
"Crocetta": { "borders" : [] },
"Fossalta": { "borders" : [] },
"Ganaceto": { "borders" : [] },
"La Rotonda": { "borders" : [] },
"Lesignana": { "borders" : [] },
"Madonnina": { "borders" : ["Cittanova"] },
"Marzaglia": { "borders" : ["Cittanova"] },
"Modena Est": { "borders" : [] },
"Navicello": { "borders" : [] },
"Paganine": { "borders" : [] },
"Ponte Alto": { "borders" : [] },
"Portile": { "borders" : [] },
"S Damaso": { "borders" : [] },
"S Donnino": { "borders" : [] },
"S Maria Mugnano": { "borders" : [] },
"S Martino Mugnano": { "borders" : [] },
"S Matteo": { "borders" : [] },
"S Pancrazio": { "borders" : [] },
"Saliceta": { "borders" : ["Cognento"] },
"Saliceto sul Panaro": { "borders" : [] },
"Torrenova": { "borders" : [] },
"Tre Olmi": { "borders" : ["Cittanova"] },
"Villaggio Zeta": { "borders" : ["Cognento"] },
"Villanova": { "borders" : [] },
}

let turn;

function assignLands(playersNum) {
	let array = Object.keys(lands);
	// Mescola l'array casualmente
	array.sort(() => Math.random() - 0.5);
  
	for (let i = 0; i < array.length; i++) {
		lands[array[i]]["team"]=i%playersNum
		lands[array[i]]["troops"]=3
	}
}

players = []
playersInGame = []

function startGame(){
	console.log("Game started")
	turn=0;
	playersInGame=[];
	assignLands(players.length);
	for(id in players){
		send(players[id],JSON.stringify({"team": id, "lands": lands}));
		playersInGame.push(players[id]);
	}
}

function onReceive(msg, peer){
	if(playersInGame[turn]!=peer){
		throw new Error("Cheat");
	}
	let obj = JSON.parse(msg);
	switch(obj["cmd"]){
		case "attack":
			onAttack(obj["from"], obj["to"], parseInt(obj["with"]));
			break;
		case "turn":
			onTurn();
			break;
	}
}

function broadcast(msg){
	playersInGame.forEach(player => {
		send(player, msg);
	});
}

function send(peer, msg){
	console.log(msg);
	peer.send(msg);
}

function onAttack(from, to, troopsAtt){
	if(lands[from]["troops"]-troopsAtt<1||troopsAtt>3){
		throw new Error("Cheat");
	}
	let troopsDef = Math.min(lands[to]["troops"], 3);

	let dicesAtt = []
	for (let i = 0; i < troopsAtt; i++) {
		dicesAtt.push(parseInt(Math.random()*6))
	}
	dicesAtt.sort();

	let dicesDef = []
	for (let i = 0; i < troopsAtt; i++) {
		dicesDef.push(parseInt(Math.random()*6))
	}
	dicesDef.sort()

	let lostAtt = 0;
	let lostDef = 0;
	for(let i = 0; i< Math.min(troopsAtt, troopsDef); i++){
		if(dicesAtt[i]>dicesDef[i]){
			lostDef++;
		}else{
			lostAtt++;
		}
	}

	lands[to]["troops"]-=lostDef;
	lands[from]["troops"]-=lostAtt;

	if(lands[to]["troops"]<1){
		lands[to]["troops"]=troopsAtt
		lands[from]["troops"]-=troopsAtt;
		lands[to]["team"]=lands[from]["team"]
	}

	broadcast(JSON.stringify({"cmd":"attack", "lands":lands}))
}

function onTurn(){
	turn = (turn+1)%playersInGame.length;
	broadcast(JSON.stringify({"cmd":"turn"}))
}

const wss = new WebSocket.Server({ port: PORT });

wss.on('connection', function connection(ws) {
	ws.on('message', function incoming(message) {
	  console.log('received: %s', message);
	  onReceive(message, ws);
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