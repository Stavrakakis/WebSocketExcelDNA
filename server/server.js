var io = require('socket.io').listen(8000);

var broadcastTime = function(){

    var date = new Date();
    io.sockets.emit('foo', { calculation: Math.random() });
}

var broadcastCalculation = function(){

    io.sockets.emit('moo', { calculation: Math.random() });
}

var broadcastFTSE = function(){

    io.sockets.emit('ftse', { calculation: Math.random() });
}

setInterval(broadcastTime, 1000);
setInterval(broadcastCalculation, 100);
setInterval(broadcastFTSE, 500);
