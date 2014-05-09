var io = require('socket.io').listen(8000);

var broadcastTime = function(){

    var date = new Date();
    io.sockets.emit('time', { time: date });
}

var broadcastCalculation = function(){

    io.sockets.emit('calculation', { calculation: Math.random() });
}

setInterval(broadcastTime, 1000);
setInterval(broadcastCalculation, 100);
