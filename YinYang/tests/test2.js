var casper = require('casper').create({
    viewportSize: {
        width: 1000,
        height: 600,
    }
});

casper.start('http://www.nuterra.tech/', function () { });

casper.then(function () {
    casper.echo('capture home.png');
    this.capture('home.png');
    this.clickLabel('Techs');
});

casper.then(function () {
    casper.echo('capture techs.png');
    this.capture('techs.png');
    this.clickLabel('Download');
});

casper.then(function () {
    casper.echo('capture download.png');
    this.capture('download.png');
});

casper.run();
// The test fails when quickly switching from the Techs tab to the download tab before the data for techs was received, in that case the techs page will overwrite the downloads page once it has been received.