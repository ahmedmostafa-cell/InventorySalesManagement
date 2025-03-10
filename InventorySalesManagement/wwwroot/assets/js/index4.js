function echart1() {
  'use strict'

  var chartdata = [
    {
      name: 'offline',
      type: 'bar',
      data: [10, 15, 5, 18, 10, 15, 22, 17, 12, 20, 15, 22]
    },
    {
      name: 'online',
      type: 'bar',
      data: [5, 12, 8, 15, 12, 25, 16, 10, 16, 9, 7, 14]
    }
  ];

  var chart = document.getElementById('echart1');
  var barChart = echarts.init(chart);

  var option = {
    grid: {
      top: '6',
      right: '0',
      bottom: '17',
      left: '25',
    },
    xAxis: {
      data: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'June', 'July', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
      axisLine: {
        lineStyle: {
          color: 'rgba(227, 237, 252,0.5)'
        }
      },
      axisLabel: {
        fontSize: 10,
        color: '#a7b4c9'
      }
    },
    tooltip: {
      show: true,
      showContent: true,
      alwaysShowContent: true,
      triggerOn: 'mousemove',
      trigger: 'axis',
      axisPointer:
      {
        label: {
          show: false,
        }
      }
    },
    yAxis: {
      splitLine: {
        lineStyle: {
          color: 'rgba(227, 237, 252,0.5)'
        }
      },
      axisLine: {
        lineStyle: {
          color: 'rgba(227, 237, 252,0.5)'
        }
      },
      axisLabel: {
        fontSize: 10,
        color: '#a7b4c9'
      }
    },
    series: chartdata,
    color: [myVarVal, myVarVal1],
    barMaxWidth: 10
  };

  barChart.setOption(option);
  window.addEventListener('resize', function () {
    barChart.resize();
  })
}