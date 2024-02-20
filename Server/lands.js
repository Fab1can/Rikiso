const lands = {
    Albareto: { borders: ["S Matteo", "Crocetta", "Torrenova", "Navicello"] },
    "Amendola Est": {
        borders: [
            "Modena Est",
            "Fossalta",
            "Collegarola",
            "La Rotonda",
            "Saliceta",
            "Amendola Ovest",
        ],
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
    },
    Baggiovara: {
        borders: [
            "Cittanova",
            "Cognento",
            "Saliceta",
            "S Maria Mugnano",
            "S Martino Mugnano",
        ],
    },
    Bruciata: { borders: ["Cittanova", "Cognento", "Amendola Ovest"] },
    Cittanova: {
        borders: [
            "Cognento",
            "Baggiovara",
            "Marzaglia",
            "Bruciata",
            "Madonnina",
            "Tre Olmi",
        ],
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
    },
    Collegarola: {
        borders: [
            "Amendola Est",
            "La Rotonda",
            "S Damaso",
            "Fossalta",
            "Portile",
        ],
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
    },
    Fossalta: {
        borders: [
            "Modena Est",
            "Amendola Est",
            "Collegarola",
            "S Damaso",
            "Saliceto sul Panaro",
        ],
    },
    Ganaceto: { borders: ["Villanova", "Lesignana"] },
    "La Rotonda": {
        borders: [
            "Amendola Est",
            "Saliceta",
            "S Maria Mugnano",
            "Portile",
            "Collegarola",
        ],
    },
    Lesignana: {
        borders: [
            "Villanova",
            "Ganaceto",
            "S Pancrazio",
            "Tre Olmi",
            "Ponte Alto",
        ],
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
    },
    Marzaglia: { borders: ["Cittanova"] },
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
    },
    Navicello: {
        borders: ["Albareto", "Modena Est", "Saliceto sul Panaro", "Torrenova"],
    },
    Paganine: { borders: ["Portile", "S Martino Mugnano"] },
    "Ponte Alto": {
        borders: [
            "S Matteo",
            "Lesignana",
            "Tre Olmi",
            "S Pancrazio",
            "Madonnina",
            "Crocetta",
        ],
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
    },
    "S Damaso": {
        borders: ["Collegarola", "Portile", "S Donnino", "Fossalta"],
    },
    "S Donnino": { borders: ["Portile", "S Damaso"] },
    "S Maria Mugnano": {
        borders: ["La Rotonda", "Portile", "Baggiovara", "S Martino Mugnano"],
    },
    "S Martino Mugnano": {
        borders: ["S Maria Mugnano", "Baggiovara", "Paganine"],
    },
    "S Matteo": {
        borders: [
            "Albareto",
            "Crocetta",
            "Ponte Alto",
            "S Pancrazio",
            "Villanova",
        ],
    },
    "S Pancrazio": {
        borders: ["S Matteo", "Villanova", "Lesignana", "Ponte Alto"],
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
    },
    "Saliceto sul Panaro": { borders: ["Modena Est", "Fossalta", "Navicello"] },
    Torrenova: { borders: ["Albareto", "Crocetta", "Modena Est", "Navicello"] },
    "Tre Olmi": {
        borders: ["Cittanova", "Lesignana", "Ponte Alto", "Madonnina"],
    },
    "Villaggio Zeta": { borders: ["Cognento", "Amendola Ovest", "Saliceta"] },
    Villanova: {
        borders: ["S Matteo", "S Pancrazio", "Lesignana", "Ganaceto"],
    },
};

let troopsTot = 40;

function getInitialPlayerTroops(playersNum, playerId){
    return (troopsTot%playersNum==0)?troopsTot/playersNum : (parseInt(troopsTot/playersNum)+(playerId<playersNum-troopsTot%playersNum)?1:0);
}

module.exports = {
    lands: lands,
    getInitialPlayerTroops: getInitialPlayerTroops
};