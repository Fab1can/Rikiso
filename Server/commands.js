const { lands, continents, getInitialPlayerTroops } = require("./lands.js");

/*
lands[land_name]={
    ...
    land_name:{
        borders:[...land_border_names...],
        continent: continent_name,
        team: team_id,
        troops: land_troops
    }
}
*/

const PHASES = {
    NOT_READY: 0,
    PRE_PLACING: 1,
    PLACING: 2,
    ATTACKING: 3,
    MOVING: 4,
    GAME_ENDED: 5,
};

const PRE_PLACING_TROOPS = 3;

let turn;
let placing;
let players = [];
let playersInGame = [];
let readyPlayers = 0;
let troopsToPlace = 0;
let prePlacingTroopsPerPlayer = [];
let gamePhase = PHASES.NOT_READY;
let continentCount = {};

function onPlace(land) {
    if (
        troopsToPlace <= 0 ||
        (gamePhase != PHASES.PRE_PLACING && gamePhase != PHASES.PLACING) ||
        lands[land]["team"] != turn
    ) {
        throw new Error("Cheat");
    }
    lands[land]["troops"]++;
    troopsToPlace--;
    if (gamePhase == PHASES.PRE_PLACING) {
        prePlacingTroopsPerPlayer[turn]--;
    }
    sendPlace(lands);
    if (troopsToPlace == 0) {
        nextPhase();
    }
}

function sendPlace(lands) {
    broadcastGameObj({ cmd: "place", lands: lands });
}

function nextPhase() {
    switch (gamePhase) {
        case PHASES.NOT_READY:
            startGame();
            break;
        case PHASES.PRE_PLACING:
            nextTurn();
            break;
        case PHASES.PLACING:
            gamePhase = PHASES.ATTACKING;
            break;
        case PHASES.ATTACKING:
            gamePhase = PHASES.MOVING;
            break;
        case PHASES.MOVING:
            nextTurn();
            break;
    }
}

function onMove(from, to, troops) {
    if (
        lands[from]["troops"] - troops < 1 ||
        troopsAtt < 0 ||
        lands[from]["team"] != turn ||
        lands[to]["team"] != turn ||
        gamePhase != PHASES.MOVING
    ) {
        throw new Error("Cheat");
    }

    lands[to]["troops"] += troops;
    lands[from]["troops"] -= troops;

    sendMove(lands);
    nextPhase();
}

function sendMove(lands) {
    broadcastGameObj({ cmd: "move", lands: lands });
}

function onAttack(from, to, troopsAtt) {
    if (
        lands[from]["troops"] - troopsAtt < 1 ||
        troopsAtt > 3 ||
        troopsAtt <= 0 ||
        lands[from]["team"] != turn ||
        lands[to]["team"] == turn ||
        gamePhase != PHASES.ATTACKING
    ) {
        throw new Error("Cheat");
    }
    let troopsDef = Math.min(lands[to]["troops"], 3);

    let dicesAtt = [];
    for (let i = 0; i < troopsAtt; i++) {
        dicesAtt.push(parseInt(Math.random() * 6));
    }
    dicesAtt.sort();
    dicesAtt.reverse();

    let dicesDef = [];
    for (let i = 0; i < troopsAtt; i++) {
        dicesDef.push(parseInt(Math.random() * 6));
    }
    dicesDef.sort();
    dicesDef.reverse();

    let lostAtt = 0;
    let lostDef = 0;
    for (let i = 0; i < Math.min(troopsAtt, troopsDef); i++) {
        if (dicesAtt[i] > dicesDef[i]) {
            lostDef++;
        } else {
            lostAtt++;
        }
    }

    lands[to]["troops"] -= lostDef;
    lands[from]["troops"] -= lostAtt;

    let conquer = false;
    if (lands[to]["troops"] < 1) {
        conquer = true;
        lands[to]["troops"] = troopsAtt;
        lands[from]["troops"] -= troopsAtt;
        lands[to]["team"] = lands[from]["team"];
    }

    sendAttack(dicesAtt, dicesDef, conquer, lands);
}

function sendAttack(dicesAtt, dicesDef, conquer, lands) {
    broadcastGameObj({
        cmd: "attack",
        dicesAtt: dicesAtt,
        dicesDef: dicesDef,
        conquer: conquer,
        lands: lands,
    });
}

function nextTurn() {
    turn = (turn + 1) % playersInGame.length;
    switch (gamePhase) {
        case PHASES.NOT_READY:
            gamePhase = PHASES.PRE_PLACING;
        case PHASES.PRE_PLACING:
            if (prePlacingTroopsPerPlayer[turn] == 0) {
                gamePhase = PHASES.PLACING;
                troopsToPlace =
                    parseInt(getTeamLandsCount(turn) / 3) +
                    getPlayerContinentTroops();
            } else {
                troopsToPlace = Math.min(
                    PRE_PLACING_TROOPS,
                    prePlacingTroopsPerPlayer[turn]
                );
            }
            break;
        case PHASES.PLACING:
        case PHASES.ATTACKING:
        case PHASES.MOVING:
            gamePhase = PHASES.PLACING;
            troopsToPlace =
                parseInt(getTeamLandsCount(turn) / 3) +
                getPlayerContinentTroops();
            break;
    }
    sendTurn(troopsToPlace, gamePhase == PHASES.PRE_PLACING);
}

function getPlayerContinentTroops() {
    let troops = 0;
    let playerContinents = {};
    for (item in lands) {
        if (lands[item].team == turn) {
            let continent = lands[item].continent;
            if (continent in playerContinents) {
                playerContinents[continent]++;
            } else {
                playerContinents[continent] = 1;
            }
            if (playerContinents[continent] == continentCount[continent]) {
                troops += continents[continent];
            }
        }
    }
    return troops;
}

function sendTurn(troopsToPlace, isPre) {
    broadcastGameObj({
        cmd: "turn",
        troops: troopsToPlace,
        pre: isPre,
    });
}

function onTurn() {
    nextTurn();
}

function onReady(isReady) {
    readyPlayers += isReady ? 1 : -1; //BISOGNA INSERIRE UN CHECK PER EVITARE CHE UN CLIENT MANDI UN VALORE FRAUDOLENTO
    if (readyPlayers == players.length && readyPlayers > 1) {
        nextPhase();
    }
}

function onPhase() {
    nextPhase();
}

function startGame() {
    console.log("Game started");
    turn = -1;
    readyPlayers = 0;
    playersInGame = [];
    assignLands(players.length);
    for (id in players) {
        send(players[id], JSON.stringify({ team: id, lands: lands }));
        playersInGame.push(players[id]);
    }
    nextTurn();
}

function assignLands(playersNum) {
    let array = Object.keys(lands);
    let landsNum = array.length;

    for (let i = 0; i < playersNum; i++) {
        let troops = getInitialPlayerTroops(playersNum, i);
        if (troops < getTeamLandsCount(i)) {
            throw new Error("Not enough troops");
        }
        prePlacingTroopsPerPlayer.push(troops);
    }

    array.sort(() => Math.random() - 0.5);

    for (let i = 0; i < landsNum; i++) {
        lands[array[i]]["team"] = i % playersNum;
        lands[array[i]]["troops"] = 1;
    }
}

function getTeamLandsCount(team) {
    let count = 0;
    for (land in lands) {
        if (lands[land]["team"] == team) count++;
    }
    return count;
}

function isInGame() {
    return playersInGame.length > 1;
}

function onReceive(msg, peer) {
    if (isInGame() && playersInGame[turn] != peer) {
        throw new Error("Cheat");
    }
    let obj = JSON.parse(msg);
    switch (obj["cmd"]) {
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
        case "phase":
            onPhase();
            break;
        case "move":
            onMove(obj["from"], obj["to"], parseInt(obj["with"]));
            break;
    }
}

function broadcastGame(msg) {
    playersInGame.forEach((player) => {
        send(player, msg);
    });
}

function broadcast(msg) {
    players.forEach((player) => {
        send(player, msg);
    });
}

function broadcastGameObj(obj) {
    broadcastGame(JSON.stringify(obj));
}

function broadcastObj(obj) {
    broadcast(JSON.stringify(obj));
}

function send(peer, msg) {
    console.log(msg.substr(0, 20));
    peer.send(msg);
}

function endGame() {
    console.log("Game stopped");
    broadcastObj({ cmd: "endgame" });
    playersInGame = [];
    gamePhase = PHASES.NOT_READY;
}

function onDisconnect(ws) {
    if (playersInGame.includes(ws)) {
        endGame();
    }
    console.log("Player disconnected");
    players = players.filter((w) => w != ws);
}

function onConnect(ws) {
    console.log("Player connected");
    players.push(ws);
}

module.exports = {
    onConnect: onConnect,
    onReceive: onReceive,
    onDisconnect: onDisconnect,
};

for (item in lands) {
    let continent = lands[item].continent;
    if (continent in continentCount) {
        continentCount[continent]++;
    } else {
        continentCount[continent] = 1;
    }
}
