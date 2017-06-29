var casper = require('casper').create({
    viewportSize: {
        width: 1000,
        height: 600,
    }
});

casper.on('remote.message', function(msg) {
	this.echo('Message: ' + msg, "INFO");
});

casper.on('page.error', function(msg, trace) {
	this.echo('Error: ' + msg, "ERROR");
	for (var i = 0; i < trace.length; i++) {
		var item = trace[i];
		this.echo('  ' + item.file + ':' + item.line, "ERROR");
	}
});

function capture(name) {
	casper.echo("capture " + name);
	casper.capture(name);
}

casper.start('http://localhost:9000/');

casper.then(function () {
    capture('home.png');
    this.clickLabel('Download');
});

casper.then(function () {
    capture('download.png');
});

casper.run();
// The test checks if the front page and the download page can be loaded. This means that bootstrap must be functional otherwise the downloads button wont load the downloads page