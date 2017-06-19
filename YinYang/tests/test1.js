var casper = require('casper').create({
    viewportSize: {
        width: 1000,
        height: 600,
    }
});

casper.start('http://www.nuterra.tech/', function () {
    casper.echo('message_1');
});

casper.then(function () {
    casper.echo('capture home.png');
    this.capture('home.png');
    this.clickLabel('Download');
});

casper.then(function () {
    casper.echo('capture download.png');
    this.capture('download.png');
});

casper.run();
// The test checks if the front page and the download page can be loaded. This means that bootstrap must be functional otherwise the downloads button wont load the downloads page