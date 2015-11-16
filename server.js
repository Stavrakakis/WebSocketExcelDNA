var io = require('socket.io').listen(8000);
var h = 1;

var broadcastTime = function(){

    var date = new Date();
    console.log('time');
    io.sockets.emit('message', {value: date});
}

var broadcastCalculation = function(){
    console.log('calculation');
    io.sockets.emit('calculation', {calculation: Math.random()});
}

var timer = setInterval(broadcastTime, 1000);
var calculator = setInterval(broadcastCalculation, 100);

io.sockets.on('connection', function (socket) {
  
  console.log('connection!');
  
});
