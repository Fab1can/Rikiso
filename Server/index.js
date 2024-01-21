const PORT = 18000
const WebSocket = require('ws');

const lands = {
	"Albareto": { "borders" : ["S Matteo","Crocetta","Torrenova","Navicello"] },
"Amendola Est": { "borders" : ["Modena Est","Fossalta","Collegarola","La Rotonda","Saliceta","Amendola Ovest"] },
"Amendola Ovest": { "borders" : ["Cognento","Amendola Est","Villaggio Zeta","Bruciata","Madonnina","Saliceta"] },
"Baggiovara": { "borders" : ["Cittanova","Cognento","Saliceta","S Maria Mugnano","S Martino Mugnano"] },
"Bruciata": { "borders" : ["Cittanova","Cognento","Amendola Ovest"] },
"Cittanova": { "borders" : ["Cognento","Baggiovara","Marzaglia","Bruciata","Madonnina","Tre Olmi"] },
"Cognento": { "borders" : ["Cittanova","Baggiovara","Villaggio Zeta","Saliceta","Amendola Ovest","Bruciata"] },
"Collegarola": { "borders" : ["Amendola Est","La Rotonda","S Damaso","Fossalta","Portile"] },
"Crocetta": { "borders" : ["Albareto","S Matteo","Ponte Alto","Madonnina","Torrenova","Modena Est"] },
"Fossalta": { "borders" : ["Modena Est","Amendola Est","Collegarola","S Damaso","Saliceto sul Panaro"] },
"Ganaceto": { "borders" : ["Villanova","Lesignana"] },
"La Rotonda": { "borders" : ["Amendola Est","Saliceta","S Maria Mugnano","Portile","Collegarola"] },
"Lesignana": { "borders" : ["Villanova","Ganaceto","S Pancrazio","Tre Olmi","Ponte Alto"] },
"Madonnina": { "borders" : ["Cittanova","Ponte Alto","Crocetta","Modena Est","Amendola Ovest","Tre Olmi"] },
"Marzaglia": { "borders" : ["Cittanova"] },
"Modena Est": { "borders" : ["Crocetta","Madonnina","Torrenova","Navicello","Saliceto sul Panaro","Fossalta","Amendola Est"] },
"Navicello": { "borders" : ["Albareto","Modena Est","Saliceto sul Panaro","Torrenova"] },
"Paganine": { "borders" : ["Portile","S Martino Mugnano"] },
"Ponte Alto": { "borders" : ["S Matteo","Lesignana","Tre Olmi","S Pancrazio","Madonnina","Crocetta"] },
"Portile": { "borders" : ["La Rotonda","Collegarola","S Damaso","S Donnino","Paganine","S Maria Mugnano"] },
"S Damaso": { "borders" : ["Collegarola","Portile","S Donnino","Fossalta"] },
"S Donnino": { "borders" : ["Portile","S Damaso"] },
"S Maria Mugnano": { "borders" : ["La Rotonda","Portile","Baggiovara","S Martino Mugnano"] },
"S Martino Mugnano": { "borders" : ["S Maria Mugnano","Baggiovara","Paganine"] },
"S Matteo": { "borders" : ["Albareto","Crocetta","Ponte Alto","S Pancrazio","Villanova"] },
"S Pancrazio": { "borders" : ["S Matteo","Villanova","Lesignana","Ponte Alto"] },
"Saliceta": { "borders" : ["Cognento","Amendola Est","Amendola Ovest","Baggiovara","La Rotonda","Villaggio Zeta"] },
"Saliceto sul Panaro": { "borders" : ["Modena Est","Fossalta","Navicello"] },
"Torrenova": { "borders" : ["Albareto","Crocetta","Modena Est","Navicello"] },
"Tre Olmi": { "borders" : ["Cittanova","Lesignana","Ponte Alto","Madonnina"] },
"Villaggio Zeta": { "borders" : ["Cognento","Amendola Ovest","Saliceta"] },
"Villanova": { "borders" : ["S Matteo","S Pancrazio","Lesignana","Ganaceto"] },
}

let turn;
let placing;
let readyPlayers = 0;
let players = []
let playersInGame = []

function assignLands(playersNum) {
	let array = Object.keys(lands);
	array.sort(() => Math.random() - 0.5);
  
	for (let i = 0; i < array.length; i++) {
		lands[array[i]]["team"]=i%playersNum
		lands[array[i]]["troops"]=3
	}
}


function startGame(){
	console.log("Game started")
	turn=-1;
	readyPlayers=0;
	playersInGame=[];
	assignLands(players.length);
	for(id in players){
		send(players[id],JSON.stringify({"team": id, "lands": lands}));
		playersInGame.push(players[id]);
	}
	onTurn();
}

function getTeamLandsCount(team){
	let count = 0;
	for(land in lands){
		if(lands[land]["team"]==team) count++;
	}
	return count;
}

function isInGame(){
	return playersInGame.length>1;
}

function onReceive(msg, peer){
	if(isInGame() && playersInGame[turn]!=peer){
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
		case "ready":
			onReady(obj["value"]);
			break;
		case "place":
			onPlace(obj["land"]);
			break;
	}
}

function onPlace(land){
	if(lands[land]["team"]!=turn){
		throw new Error("Cheat");
	}
	lands[land]["troops"]++;
	broadcastGameObj({"cmd":"place", "lands":lands});
}

function broadcastGame(msg){
	playersInGame.forEach(player => {
		send(player, msg);
	});
}

function broadcast(msg){
	players.forEach(player => {
		send(player, msg);
	});
}

function broadcastGameObj(obj){
	broadcastGame(JSON.stringify(obj));
}

function broadcastObj(obj){
	broadcast(JSON.stringify(obj));
}

function send(peer, msg){
	//console.log(msg);
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
		lands[to]["team"]=lands[from]["team"];
	}

	broadcastGameObj({"cmd":"attack", "lands":lands});
}

function onTurn(){
	turn = (turn+1)%playersInGame.length;
	broadcastGameObj({"cmd":"turn", "troops":parseInt(getTeamLandsCount(turn)/3)});
}

function onReady(value){
	readyPlayers+=value?1:-1; //BISOGNA INSERIRE UN CHECK PER EVITARE CHE UN CLIENT MANDI UN VALORE FRAUDOLENTO
	if(readyPlayers==players.length&&readyPlayers>1){
		startGame();
	}
}

function endGame(){
	console.log("Game stopped")
	broadcastObj({"cmd":"endgame"});
	playersInGame=[]
}

function onDisconnect(ws){
	if(playersInGame.includes(ws)){
		endGame();
	}
	console.log("Player disconnected");
	players=players.filter((w)=>w!=ws);
}

const wss = new WebSocket.Server({ port: PORT });

wss.on('connection', function connection(ws) {
	ws.on('message', function incoming(message) {
	  console.log('received: %s', message);
	  onReceive(message, ws);
	});
	ws.on('close', function() {
		onDisconnect(ws);
    });

	console.log("Player connected");
	players.push(ws);
});



console.log("Server started on "+PORT)