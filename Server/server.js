var app = require('express')();
var server = require('http').Server(app);
var io = require('socket.io')(server);

server.listen(5055);

var enemies = [];
var playerSpawnPoints = [];
var clients = [];

io.on('connection', function (socket) {

    var currentPlayer = {};
    currentPlayer.name = 'null';

    socket.on('player connect', function () {
        for (var i = 0; i < clients.length; i++) {
            var playerConnected = {
                name: clients[i].name,
                position: clients[i].position,
                rotation: clients[i].rotation
            };

            socket.emit('other player connected', playerConnected);
        }
    });

    socket.on('play', function (data) {
        console.log(data.name + ' connected');
        console.log((clients.length + 1) + ' total players');

        if (clients.length === 0) {
            numberOfEnemies = data.enemySpawnPoints.length;
            enemies = [];
            data.enemySpawnPoints.forEach(function (enemySpawnPoint) {
                var enemy = {
                    name: guid(),
                    position: enemySpawnPoint.position
                };
                enemies.push(enemy);
            });
            playerSpawnPoints = [];
            data.playerSpawnPoints.forEach(function (_playerSpawnPoint) {
                var playerSpawnPoint = {
                    position: _playerSpawnPoint.position,
                    rotation: _playerSpawnPoint.rotation
                };
                playerSpawnPoints.push(playerSpawnPoint);
            });
        }

        var enemiesResponse = {
            enemies: enemies
        };

        socket.emit('enemies', enemiesResponse);
        var randomSpawnPoint = playerSpawnPoints[Math.floor(Math.random() * playerSpawnPoints.length)];
        currentPlayer = {
            name: data.name,
            position: randomSpawnPoint.position,
            rotation: randomSpawnPoint.rotation
        };
        clients.push(currentPlayer);

        socket.emit('play', currentPlayer);
        
        socket.broadcast.emit('other player connected', currentPlayer);
    });

    socket.on('player move', function (data) {
        currentPlayer.position = data.position;
        socket.broadcast.emit('player move', currentPlayer);
    });

    socket.on('player rotate', function (data) {
        currentPlayer.rotation = data.rotation;
        socket.broadcast.emit('player rotate', currentPlayer);
    });

    socket.on('player shoot', function () {
        var data = {
            name: currentPlayer.name
        };
        socket.emit('player shoot', data);
        socket.broadcast.emit('player shoot', data);
    });

    socket.on('disconnect', function(){
        console.log(currentPlayer.name + ' disconnected');
        console.log((clients.length - 1) + ' total players');
        socket.broadcast.emit('other player disconnected', currentPlayer);
        for (var i = 0; i < clients.length; i++){
            if (clients[i].name === currentPlayer.name){
                clients.splice(i, 1);
            }
        }
    });
});

console.log('server is running...');
function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
}