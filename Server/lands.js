const lands = {
    Albareto: {
        borders: ["S Matteo", "Crocetta", "Torrenova", "Navicello"],
        continent: "Azzurro",
    },
    "Amendola Est": {
        borders: [
            "Modena Est",
            "Fossalta",
            "Collegarola",
            "La Rotonda",
            "Saliceta",
            "Amendola Ovest",
        ],
        continent: "Bianco",
    },
    "Amendola Ovest": {
        borders: [
            "Cognento",
            "Amendola Est",
            "Villaggio Zeta",
            "Bruciata",
            "Madonnina",
            "Saliceta",
        ],
        continent: "Bianco",
    },
    Baggiovara: {
        borders: [
            "Cittanova",
            "Cognento",
            "Saliceta",
            "S Maria Mugnano",
            "S Martino Mugnano",
        ],
        continent: "Giallo",
    },
    Bruciata: {
        borders: ["Cittanova", "Cognento", "Amendola Ovest"],
        continent: "Bianco",
    },
    Cittanova: {
        borders: [
            "Cognento",
            "Baggiovara",
            "Marzaglia",
            "Bruciata",
            "Madonnina",
            "Tre Olmi",
        ],
        continent: "Aqua",
    },
    Cognento: {
        borders: [
            "Cittanova",
            "Baggiovara",
            "Villaggio Zeta",
            "Saliceta",
            "Amendola Ovest",
            "Bruciata",
        ],
        continent: "Rosso",
    },
    Collegarola: {
        borders: [
            "Amendola Est",
            "La Rotonda",
            "S Damaso",
            "Fossalta",
            "Portile",
        ],
        continent: "Celeste",
    },
    Crocetta: {
        borders: [
            "Albareto",
            "S Matteo",
            "Ponte Alto",
            "Madonnina",
            "Torrenova",
            "Modena Est",
        ],
        continent: "Bianco",
    },
    Fossalta: {
        borders: [
            "Modena Est",
            "Amendola Est",
            "Collegarola",
            "S Damaso",
            "Saliceto sul Panaro",
        ],
        continent: "Celeste",
    },
    Ganaceto: { borders: ["Villanova", "Lesignana"], continent: "Lime" },
    "La Rotonda": {
        borders: [
            "Amendola Est",
            "Saliceta",
            "S Maria Mugnano",
            "Portile",
            "Collegarola",
        ],
        continent: "Bianco",
    },
    Lesignana: {
        borders: [
            "Villanova",
            "Ganaceto",
            "S Pancrazio",
            "Tre Olmi",
            "Ponte Alto",
        ],
        continent: "Lime",
    },
    Madonnina: {
        borders: [
            "Cittanova",
            "Ponte Alto",
            "Crocetta",
            "Modena Est",
            "Amendola Ovest",
            "Tre Olmi",
        ],
        continent: "Bianco",
    },
    Marzaglia: { borders: ["Cittanova"], continent: "Aqua" },
    "Modena Est": {
        borders: [
            "Crocetta",
            "Madonnina",
            "Torrenova",
            "Navicello",
            "Saliceto sul Panaro",
            "Fossalta",
            "Amendola Est",
        ],
        continent: "Bianco",
    },
    Navicello: {
        borders: ["Albareto", "Modena Est", "Saliceto sul Panaro", "Torrenova"],
        continent: "Azzurro",
    },
    Paganine: { borders: ["Portile", "S Martino Mugnano"], continent: "Viola" },
    "Ponte Alto": {
        borders: [
            "S Matteo",
            "Lesignana",
            "Tre Olmi",
            "S Pancrazio",
            "Madonnina",
            "Crocetta",
        ],
        continent: "Bianco",
    },
    Portile: {
        borders: [
            "La Rotonda",
            "Collegarola",
            "S Damaso",
            "S Donnino",
            "Paganine",
            "S Maria Mugnano",
        ],
        continent: "Viola",
    },
    "S Damaso": {
        borders: ["Collegarola", "Portile", "S Donnino", "Fossalta"],
        continent: "Celeste",
    },
    "S Donnino": { borders: ["Portile", "S Damaso"], continent: "Celeste" },
    "S Maria Mugnano": {
        borders: ["La Rotonda", "Portile", "Baggiovara", "S Martino Mugnano"],
        continent: "Viola",
    },
    "S Martino Mugnano": {
        borders: ["S Maria Mugnano", "Baggiovara", "Paganine"],
        continent: "Viola",
    },
    "S Matteo": {
        borders: [
            "Albareto",
            "Crocetta",
            "Ponte Alto",
            "S Pancrazio",
            "Villanova",
        ],
        continent: "Azzurro",
    },
    "S Pancrazio": {
        borders: ["S Matteo", "Villanova", "Lesignana", "Ponte Alto"],
        continent: "Lime",
    },
    Saliceta: {
        borders: [
            "Cognento",
            "Amendola Est",
            "Amendola Ovest",
            "Baggiovara",
            "La Rotonda",
            "Villaggio Zeta",
        ],
        continent: "Bianco",
    },
    "Saliceto sul Panaro": {
        borders: ["Modena Est", "Fossalta", "Navicello"],
        continent: "Azzurro",
    },
    Torrenova: {
        borders: ["Albareto", "Crocetta", "Modena Est", "Navicello"],
        continent: "Bianco",
    },
    "Tre Olmi": {
        borders: ["Cittanova", "Lesignana", "Ponte Alto", "Madonnina"],
        continent: "Lime",
    },
    "Villaggio Zeta": {
        borders: ["Cognento", "Amendola Ovest", "Saliceta"],
        continent: "Bianco",
    },
    Villanova: {
        borders: ["S Matteo", "S Pancrazio", "Lesignana", "Ganaceto"],
        continent: "Lime",
    },
};

const continents = {
    Aqua: 2,
    Rosso: 1,
    Giallo: 0,
    Lime: 3,
    Azzurro: 3,
    Celeste: 3,
    Viola: 3,
    Bianco: 6,
};

let troopsTot = 20;

function getInitialPlayerTroops(playersNum, playerId) {
    return troopsTot % playersNum == 0
        ? troopsTot / playersNum
        : parseInt(troopsTot / playersNum) +
          (playerId < playersNum - (troopsTot % playersNum))
        ? 1
        : 0;
}

module.exports = {
    lands: lands,
    continents: continents,
    getInitialPlayerTroops: getInitialPlayerTroops,
};
