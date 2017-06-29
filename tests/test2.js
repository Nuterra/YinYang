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
    this.clickLabel('Techs');
});

casper.then(function () {
    capture('techs.png');
    this.clickLabel('Download');
});

casper.then(function () {
    capture('download.png');
    this.wait(5000);
});

casper.on('resource.requested', function (res) {
    if (/imgur\.com/.test(res.url)) {
        this.echo('failure: we loaded something from imgur');
    }
});

casper.run();
