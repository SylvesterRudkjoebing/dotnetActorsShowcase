window.createActorGraph = (graphData) => {
    var ctx = document.getElementById('actorGraph').getContext('2d');

    var nodes = graphData.nodes.map(function (node) {
        return { id: node.id, label: node.label };
    });

    var edges = graphData.edges.map(function (edge) {
        return { from: edge.from, to: edge.to, label: edge.movieTitle };
    });

    var chartData = {
        datasets: [{
            label: 'Actor Connections',
            data: nodes.map(node => ({
                x: Math.random() * 400, // Random positioning for simplicity
                y: Math.random() * 400,
                label: node.label,
                movieTitle: edge.movieTitle
            })),
        }]
    };

    var config = {
        type: 'network',
        data: chartData,
        options: {
            responsive: true,
            interaction: {
                mode: 'nearest',
                intersect: false,
            },
        }
    };

    // Initialize Chart.js and render the graph
    var actorGraph = new Chart(ctx, config);
};

