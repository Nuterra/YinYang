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
	casper.echo('capture ' + name);
	casper.capture(name);
}

casper.start('http://localhost:9000/');

casper.then(function() {
	capture('home.png');
	this.clickLabel('My Account');
	this.waitFor(function() {
		return /app\/account/.test(this.getCurrentUrl());
	}, function then(){
		this.wait(1000, function() {
			this.click('a.btn-steam');
		});
	}, function timeout() {
		this.echo("failure: did not land on app//account");
		this.exit();
	});
});

casper.then(function() {
	capture('steam.png');
	this.waitFor(function() {
		return /steamcommunity\.com/.test(this.getCurrentUrl());
	}, function then(){}, function timeout() {
		casper.echo('timeout, we ended up on ' + this.getCurrentUrl());
	});
});

casper.then(function() {
	if (/steamcommunity\.com/.test(this.getCurrentUrl())) {
		casper.echo('passed: we landed on steam');
	} else {
		casper.echo('failure: we are not on steam');
	}
});

casper.run();
