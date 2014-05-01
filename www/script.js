var width = 700,
    height = 700;

var barWidth = 50;
var barScale = 50;

var fill = d3.scale.category10();

var magnify = 70;

//constant for shifting bubble to the left
var LEFT_SHIFT = 100;

displayColorScale();

var svg = d3.select("#container").append("svg");

var bars = d3.select("#container").append("svg");

var tooltip = bars.append("text")
                .style("visibility", "visible")
                .attr("x", 50)
                .attr("y", 50);

var lastSelected;


d3.tsv("../outputPhase2.txt", type, function(error, data) {

  var ramp = d3.scale.linear().domain([d3.min(data, function(d) { return d.mean; }), 0,
                                      d3.max(data, function(d) { return d.mean; })]).range(["red","white","blue"]);

  var force = d3.layout.force()
      .nodes(data)
      .charge(function(d) { return -Math.pow(d.abs * magnify, 2.0) / 6;})
      .size([width, height - 200])
      .on("tick", tick)
      .start();

  var node = svg.selectAll(".node")
      .data(data)
    .enter().append("circle")
      .attr("class", "node")
      .attr("cx", function(d) { return d.x; })
      .attr("cy", function(d) { return d.y; })
      .attr("r", function(d) { return d.abs * magnify; })
      .style("fill", function(d) { return ramp(d.mean); })
      .style("stroke", function(d) { return "black"; })
      .call(force.drag)
      .on("mousedown", function() { d3.event.stopPropagation(); })
      .on("click", function(d){
        d3.select(this).style("stroke-width", "2.5");
        tooltip.text(d.Set);

        showBar(d, this);
      });

  svg.style("opacity", 1e-6)
    .transition()
      .duration(1000)
      .style("opacity", 1);

  function tick(e) {

    // Push nodes into the cluster
    var k = 6 * e.alpha;
    data.forEach(function(o, i) {
      o.y += k;
      o.x += k;
    });

    node.attr("cx", function(d) { return d.x - LEFT_SHIFT; })
        .attr("cy", function(d) { return d.y; });
  }

});


function type(d) {
  d.Sunday = +d.Sunday;
  d.Monday = +d.Monday;
  d.Tuesday = +d.Tuesday;
  d.Wednesday = +d.Wednesday;
  d.Thursday = +d.Thursday;
  d.Friday = +d.Friday;
  d.Saturday = +d.Saturday;

  d.mean = (d.Sunday + d.Monday + d.Tuesday + d.Wednesday + d.Thursday + d.Friday + d.Saturday) / 7;
  d.abs = (Math.abs(d.Sunday) + Math.abs(d.Monday) + Math.abs(d.Tuesday) + Math.abs(d.Wednesday) + Math.abs(d.Thursday) + Math.abs(d.Friday) + Math.abs(d.Saturday)) / 7;

  return d;
}


function showBar(d, lastOne) {
 
  if(typeof lastSelected != 'undefined') {
    d3.select(lastSelected).style("stroke-width", "1");        
  }
 
  lastSelected = lastOne;
        //hide bar thing
  d3.select("g").remove();

  var barGroup = bars.append("g");

  var suHeight = d.Sunday * barScale;
  var mHeight = d.Monday * barScale;
  var tHeight = d.Tuesday * barScale;
  var wHeight = d.Wednesday * barScale;
  var thHeight = d.Thursday * barScale;
  var fHeight = d.Friday * barScale;
  var saHeight = d.Saturday * barScale;

  barGroup.append("rect")
    .attr("width", barWidth)
    .attr("height", Math.abs(suHeight))
    .attr("x", (barWidth + 1) * 0)
    .attr("y", (suHeight>=0)?(height / 2 - suHeight):(height / 2))
    .attr("fill", "blue");

  barGroup.append("rect")
    .attr("width", barWidth)
    .attr("height", Math.abs(mHeight))
    .attr("x", (barWidth + 1) * 1)
    .attr("y", (mHeight>=0)?(height / 2 - mHeight):(height / 2))
    .attr("fill", "blue");

  barGroup.append("rect")
    .attr("width", barWidth)
    .attr("height", Math.abs(tHeight))
    .attr("x", (barWidth + 1) * 2)
    .attr("y", (tHeight>=0)?(height / 2 - tHeight):(height / 2))
    .attr("fill", "blue");

  barGroup.append("rect")
    .attr("width", barWidth)
    .attr("height", Math.abs(wHeight))
    .attr("x", (barWidth + 1) * 3)
    .attr("y", (wHeight>=0)?(height / 2 - wHeight):(height / 2))
    .attr("fill", "blue");

  barGroup.append("rect")
    .attr("width", barWidth)
    .attr("height", Math.abs(thHeight))
    .attr("x", (barWidth + 1) * 4)
    .attr("y", (thHeight>=0)?(height / 2 - thHeight):(height / 2))
    .attr("fill", "blue");

  barGroup.append("rect")
    .attr("width", barWidth)
    .attr("height", Math.abs(fHeight))
    .attr("x", (barWidth + 1) * 5)
    .attr("y", (fHeight>=0)?(height / 2 - fHeight):(height / 2))
    .attr("fill", "blue");

  barGroup.append("rect")
    .attr("width", barWidth)
    .attr("height", Math.abs(saHeight))
    .attr("x", (barWidth + 1) * 6)
    .attr("y", (saHeight>=0)?(height / 2 - saHeight):(height / 2))
    .attr("fill", "blue");
}


function displayColorScale() {
  var colorScale = d3.scale.linear()
                        .domain([0, width / 2, width])
                        .range(["red", "white", "blue"]);

  var space = d3.select("#colorBar")
                    .data([{interpolate: d3.interpolateRgb}])
                    .style("width", width + "px")
                    .style("height", "30px");

  space.append("canvas")
          .attr("width", width)
          .attr("height", 1)
          .style("width", width + "px")
          .style("height", "30px")
          .each(render);

  var spaceLabel = space.append("div");
  spaceLabel.append("p")
      .style("float", "left")
      .text("Negative");
  spaceLabel.append("p")
      .style("float", "right")
      .text("Positive");

  function render(d) {
    var context = this.getContext("2d"),
    image = context.createImageData(width, 1);
    colorScale.interpolate(d.interpolate);
    for (var i = 0, j = -1, c; i < width; ++i) {
      c = d3.rgb(colorScale(i));
      image.data[++j] = c.r;
      image.data[++j] = c.g;
      image.data[++j] = c.b;
      image.data[++j] = 255;
    }
    context.putImageData(image, 0, 0);
  }
}
