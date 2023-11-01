// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function toggleMenu() {
    var x = document.getElementById("menu");
    if (x.className === "menu") {
        x.className += " responsive";
    } else {
        x.className = "menu";
    }
}

function toggleMenu2() {
    var menu_height = document.getElementById("menu").style.height;

    if (menu_height == "0px" || menu_height == 0) {
        document.getElementById("menu").style.height = "170px";
    }
    else {
        document.getElementById("menu").style.height = "0px";
    }
}

function selectPage(menuitem) {
    //document.getElementById("menu_sidepanel").style.height = "0";
    //document.getElementById("navbar_header").innerHTML = page;
    const menuitems = ["menu1", "menu2","menu3"];
    let i = 0;

    for (i = 0; i < menuitems.length; i++) {
        if (menuitem == menuitems[i]) {
            document.getElementById(menuitem).className = "selected";
        }
        else {
            document.getElementById(menuitems[i]).className = "unselected";
        }
    }
}

function drawHandicapGraph(handicapData, graphHeight, graphWidth) {
    var graphCanvas = document.getElementById("graph");
    var graphCtx = graphCanvas.getContext("2d");
    var tipCanvas = document.getElementById("toolTip");
    var tipCtx = tipCanvas.getContext("2d");
    //var graphHeight = 300;
    //var graphWidth = 600;
    var graphTopMargin = 20;
    var graphBottomMargin = 60;
    var graphLeftMargin = 46;
    var graphRightMargin = 46;
    var dotRadius = 5;
    var graphDetails = getGraphDetails(handicapData, graphHeight, graphWidth, graphLeftMargin, graphTopMargin);

    graphCanvas.height = graphDetails.graphHeight + graphTopMargin + graphBottomMargin;
    graphCanvas.width = graphDetails.graphWidth + graphLeftMargin + graphRightMargin;

    //Draw Grid
    graphCtx.beginPath();
    graphCtx.strokeStyle = '#CCC';
    graphCtx.font = "12px Arial";

    let yLable = graphDetails.max;
    yPos = graphTopMargin;

    //Draw Grid Lines
    for (var i = 0; i < graphDetails.yLines.length; i++) {
        graphCtx.moveTo(graphDetails.yLines[i].x1, graphDetails.yLines[i].y1);
        graphCtx.lineTo(graphDetails.yLines[i].x2, graphDetails.yLines[i].y2);
        graphCtx.fillText(graphDetails.yLines[i].lable, 15, graphDetails.yLines[i].y1 + 3);
    }

    graphCtx.stroke();
    graphCtx.closePath();

    // Draw graph xaxis markers
    graphCtx.beginPath();

    for (var i = 0; i < graphDetails.xMarkers.length; i++) {
        graphCtx.moveTo(graphDetails.xMarkers[i].x1, graphDetails.xMarkers[i].y1);
        graphCtx.lineTo(graphDetails.xMarkers[i].x2, graphDetails.xMarkers[i].y2);
    }
    graphCtx.stroke();
    graphCtx.closePath();

    //Draw x axis lables
    graphCtx.beginPath();

    for (var i = 0; i < graphDetails.xLabels.length; i++) {
        graphCtx.save();
        graphCtx.translate(graphDetails.xLabels[i].x, graphDetails.xLabels[i].y);
        graphCtx.rotate(25 * Math.PI / 180);
        graphCtx.textAlign = "center";
        graphCtx.fillText(graphDetails.xLabels[i].label, 20, 0);
        graphCtx.restore();
    }

    graphCtx.fillStyle = 'orange';
    graphCtx.strokeStyle = '#FFF';
    tipCtx.font = "12px Arial";
    tipCtx.fillStyle = 'white';

    //Draw Graph Dots
    for (var i = 0; i < graphDetails.dots.length; i++) {
        handicapIndex = handicapData[i];
        graphCtx.beginPath();
        graphCtx.arc(graphDetails.dots[i].x, graphDetails.dots[i].y, dotRadius, 0, 2 * Math.PI);
        graphCtx.fill();
        graphCtx.closePath();
    }

    graphCtx.strokeStyle = '#BBB';
    graphCtx.closePath();
    graphCtx.beginPath();

    //Draw Graph Lines
    for (var i = 0; i < graphDetails.lines.length; i++) {
        graphCtx.moveTo(graphDetails.lines[i].x1, graphDetails.lines[i].y1);
        graphCtx.lineTo(graphDetails.lines[i].x2, graphDetails.lines[i].y2);
        graphCtx.stroke();
    }

    //Display tooltip if dot hit
    $("#graph").mousemove(function (e) { handleMouseMove(e); });

    function handleMouseMove(e) {
        var mouseX = parseInt(e.clientX) - graphCanvas.offsetLeft;
        var mouseY = parseInt(e.clientY) - graphCanvas.offsetTop;
        var hit = false;
        let leftTip = 0;
        let topTip = 0;

        for (var i = 0; i < graphDetails.dots.length; i++) {
            var dot = graphDetails.dots[i];
            var dx = mouseX - dot.x;
            var dy = mouseY - dot.y;

            if (mouseX > dot.x - dotRadius - 5 && mouseX < dot.x + dotRadius + 5) {
                if (i == graphDetails.dots.length - 1) {
                    leftTip = dot.x - 26 + graphCanvas.offsetLeft;
                    topTip = dot.y - 50 + graphCanvas.offsetTop;
                }
                else {
                    leftTip = dot.x + 5 + graphCanvas.offsetLeft;
                    topTip = dot.y - 50 + graphCanvas.offsetTop;
                }
                tipCanvas.style.left = leftTip.toString() + 'px';
                tipCanvas.style.top = topTip.toString() + 'px';
                tipCanvas.style.backgroundColor = "#777";
                tipCtx.clearRect(0, 0, tipCanvas.width, tipCanvas.height);
                tipCtx.font = "16px Arial";
                tipCtx.fillText(dot.tipHc, 6, 20);
                tipCtx.font = "10px Arial";
                tipCtx.fillText(dot.tipDate, 7, 36);
                hit = true;
            }
            if (!hit) { tipCanvas.style.left = '-200px'; }
        }
    }
}

function getGraphDetails(handicapData, graphHeight, graphWidth, graphLeftMargin, graphTopMargin) {
    var yLines = [];
    var dots = [];
    var lines = [];
    var xMarkers = [];
    var xLabels = [];
    var max = -100;
    var min = 100;
    var maxLines = 5;
    var xDateSequence = 1; // graph x label sequence
    var yNumSequence = 1; // graph line number sequence
    var skipFirst = false; // skip first result 
    var numHorizontalLines = 0;
    var x1;
    var y1;
    var x2;
    var y2;
    var yLable;
    var line;

    // Set graph max
    for (var i = 0; i < handicapData.length; i++) {
        if (max < handicapData[i].hc)
            max = handicapData[i].hc;
    }

    max = Math.ceil(max);
    max += 1;

    // Set graph min
    for (var i = 0; i < handicapData.length; i++) {
        if (min > handicapData[i].hc)
            min = handicapData[i].hc;
    }

    min = Math.floor(min);
    min -= 1;

    // Y lable number sequence
    while ((max - min) / yNumSequence > maxLines) {
        yNumSequence++;
    }

    while (!Number.isInteger((max - min) / yNumSequence)) {
        min--;
    }

    if (handicapData.length > 10) {
        xDateSequence = 2;

        if (handicapData.length % 2 == 0) {
            skipFirst = true;
        }
    }

    // Number of horizontal grid lines    
    numHorizontalLines = ((max - min) / yNumSequence) + 1;

    // Adjust graphHeight 
    graphHeight = (max - min) * Math.floor(graphHeight / (max - min));

    // Adjust graphWidth
    graphWidth = (Math.floor(graphWidth / (handicapData.length - 1))) * (handicapData.length - 1);

    // yLines
    x1 = graphLeftMargin;
    x2 = graphLeftMargin + graphWidth;

    for (var i = 0; i < numHorizontalLines; i++) {
        y1 = graphTopMargin + (((graphHeight / (max - min)) * yNumSequence) * i);
        y2 = y1;
        lable = max - (i * yNumSequence);
        yLines.push({ x1: x1, y1: y1, x2: x2, y2: y2, lable: lable });
    }

    // dots
    for (var i = 0; i < handicapData.length; i++) {
        if (skipFirst) {
            if (i != 0) {
                x1 = graphLeftMargin + ((graphWidth / (handicapData.length - 2)) * (i - 1));
                y1 = ((max - handicapData[i].hc) * (graphHeight / (max - min))) + graphTopMargin;
                dots.push({ x: x1, y: y1, tipHc: handicapData[i].hc.toFixed(1), tipDate: handicapData[i].date });
            }
        }
        else {
            x1 = graphLeftMargin + ((graphWidth / (handicapData.length - 1)) * i);
            y1 = ((max - handicapData[i].hc) * (graphHeight / (max - min))) + graphTopMargin;
            dots.push({ x: x1, y: y1, tipHc: handicapData[i].hc.toFixed(1), tipDate: handicapData[i].date });
        }
    }

    // graph lines
    for (var i = 0; i < dots.length - 1; i++) {
        line = getLine(dots[i].x, dots[i].y, dots[i + 1].x, dots[i + 1].y);
        lines.push({ x1: line[0], y1: line[1], x2: line[2], y2: line[3] });
    }

    // x axis markers
    y1 = graphTopMargin + graphHeight;
    y2 = y1 + 10;

    for (var i = 0; i < handicapData.length; i++) {
        if (xDateSequence == 1) {
            x1 = graphLeftMargin + ((graphWidth / (handicapData.length - 1)) * i);
            x2 = x1;
        }

        if (xDateSequence == 2) {
            if (skipFirst) {
                if (i != 0) {
                    if ((i - 1) % 2 == 0) {
                        x1 = graphLeftMargin + ((graphWidth / (handicapData.length - 2)) * (i - 1));
                        x2 = x1;
                    }
                }
            }
            else {
                if (i % 2 == 0) {
                    x1 = graphLeftMargin + ((graphWidth / (handicapData.length - 1)) * i);
                    x2 = x1;
                }
            }
        }
        xMarkers.push({ x1: x1, y1: y1, x2: x2, y2: y2 })
    }

    // x axis labels
    y1 = graphTopMargin + graphHeight + 25;

    for (var i = 0; i < handicapData.length; i++) {
        if (xDateSequence == 1) {
            x1 = graphLeftMargin + ((graphWidth / (handicapData.length - 1)) * i) - 5;
            xLabels.push({ x: x1, y: y1, label: handicapData[i].date });
        }

        if (xDateSequence == 2) {
            if (skipFirst) {
                if (i != 0) {
                    if ((i - 1) % 2 == 0) {
                        x1 = graphLeftMargin + (graphWidth / (handicapData.length - 2) * (i - 1)) - 5;
                        xLabels.push({ x: x1, y: y1, label: handicapData[i].date });
                    }
                }
            }
            else {
                if (i % 2 == 0) {
                    x1 = graphLeftMargin + (graphWidth / (handicapData.length - 1) * i) - 5;
                    xLabels.push({ x: x1, y: y1, label: handicapData[i].date });
                }
            }
        }
    }

    return { yLines: yLines, dots: dots, lines: lines, xMarkers: xMarkers, xLabels: xLabels, graphHeight: graphHeight, graphWidth: graphWidth };
}

function getLine(x1, y1, x2, y2) {
    var x = x2 - x1;
    var y = 0;
    var line = [];

    if (y1 < y2) {
        y = y2 - y1;
    }
    else {
        y = y1 - y2;
    }

    var angle = Math.atan(y / x);

    var j = Math.cos(angle) * 5;
    var k = Math.sin(angle) * 5;

    line.push(x1 + j);

    if (y1 < y2) {
        line.push(y1 + k);
    }
    else {
        line.push(y1 - k);
    }

    line.push(x2 - j);

    if (y1 < y2) {
        line.push(y2 - k);
    }
    else {
        line.push(y2 + k);
    }

    return line;
}

