var io = require('socket.io').listen(8000);

var broadcast = function(feed){

    io.sockets.emit(feed, { calculation: Math.random() });
}

setInterval(broadcast.bind(null, "nasdaq"),  1000);
setInterval(broadcast.bind(null, "ftse"),  100);
setInterval(broadcast.bind(null, "dowjones"),  500);
