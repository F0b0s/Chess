﻿var board = null;
var id;
var timerId;

$(document).ready(function () {
    $("#depthSpinner").spinner();
    $("#depthSpinner").spinner('value', 1);
    $("#outputLinesSpinner").spinner();
    $("#outputLinesSpinner").spinner('value', 1);
    initializeBoard();
});

function makeMove(position, caller) {
    $('.move').removeClass('currentMove');
    $(caller).addClass('currentMove');
    board.position(position, true);
}

function initializeBoard() {
    var cfg = {
        showNotation: true,
        position: 'rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1',
        onChange: onPositionChange,
        sparePieces: true
    };
    board = new ChessBoard('board', cfg);
    board.showErrors = 'console';
    $('#fen').val(cfg.position);
}

function createFenString() {
    var positionFen = board.fen();
    var moveTurn = $('.moveTurn:checked').val();
    var fen = positionFen + ' ' + moveTurn + ' ';
    if ($('#w00').prop('checked'))  {
        fen += 'K';
    }

    if ($('#w000').prop('checked')) {
        fen += 'Q';
    }

    if ($('#b00').prop('checked')) {
        fen += 'k';
    }

    if ($('#b000').prop('checked')) {
        fen += 'q';
    }

    fen += ' - 0 1';
    return fen;
}

function moveTurnChanged() {
    var fenString = createFenString();
    $('#fen').val(fenString);
}

function onPositionChange() {
    var fenString = createFenString();
    $('#fen').val(fenString);
}

function timer() {
    var data = {
        guid: id
    }
    $.get('/Home/GetOutput', data)
        .done(handleSuccessSuccess)
        .done(handleEngineAnalysis)
        .fail(handleServerError);
}

function handleEngineAnalysis(analysisContainer) {
    if (analysisContainer.Completed == true) {
        clearInterval(timerId);
    }

    var cfg = {
        position: board.fen()
    };

    $('#engineOutput').show();
    $('#analysisSettings').hide();

    var headerRow = $('<tr><td>Evaluation</td><td>Line</td></tr>');
    $('#engineTable').empty();
    $('#engineTable').append(headerRow);

    var lines = analysisContainer.PositionAnalysis.Lines;
    for (var j = 0; j < lines.length; j++) {
        var cell = $('<td/>');
        var fakeBoard = new ChessBoard('fakeBoard', cfg);
        var lineInfo = lines[j];

        var moves = lineInfo.Moves.split(' ');
        var initialPosition = board.fen();
        $('<a class="move" hidden="true" href="#"></a>').appendTo(cell).click(function () { makeMove(initialPosition, this); });
        
        for (var i = 0; i < moves.length; i++) {
            (function(n) {
                var move = moves[n];
                fakeBoard.move(move.slice(0, 2) + '-' + move.slice(-2));
                var currentPosition = fakeBoard.fen();
                if (n % 2 == 0) {
                    var currentMove;
                    if (n == 0 && $('.moveTurn:checked').val() === 'b') {
                        currentMove = n / 2 + 1 + '... ';
                    } else {
                        currentMove = n / 2 + 1 + '. ';
                    }
                    cell.append($(' <b>' + currentMove + '</b>'));
                }
                $('<a class="move" href="#">' + move + ' </a>')
                    .appendTo(cell)
                    .click(function() { makeMove(currentPosition, this); });
            })(i);
        }

        var row = $('<tr/>')
            .append('<td>' + lineInfo.Score / 100 + '</td>')
            .append(cell);
        $('#engineTable').append(row);
    }

    var info = analysisContainer.PositionAnalysis;
    var infoRow = '<tr><td colspan="2">' + info.EngneInfo + ', ' + info.AnalysisStatistics.Time / 1000 + ' sec, depth ' + info.AnalysisStatistics.Depth + ', ' + info.AnalysisStatistics.Nodes + ' nodes' + '</td></tr>';
    $('#engineTable').append(infoRow);
    makeMove(board.fen(), $('#engineTable').find('a').first());
    $('#analysisNavigation').show();
    $('#startNewAnalysisBtn').show();
}

function analyze() {
    if (board.fen() == '8/8/8/8/8/8/8/8') {
        alert('Set position for analysis!');
        return;
    }

    $('#positionNavigation').hide();
    
    var data = {
        fen: $('#fen').val(),
        depth: $('#depthSpinner').spinner('value'),
        outputLines: $('#outputLinesSpinner').val()
    }

    $.get('/Home/StartAnalyze', data)
        .done(handleSuccessSuccess)
        .done(startPolling)
        .fail(handleServerError);
}

function startPolling(response) {
    id = response;
    timerId = setInterval(timer, 500);

    var cfg = {
        draggable: false,
        position: board.fen(),
        onChange: onPositionChange,
    };

    board = new ChessBoard('board', cfg);
}

function handleSuccessSuccess() {
    $('.error').hide();
}

function handleServerError() {
    if (timerId != undefined) {
        clearInterval(timerId);
    }

    $('.error').empty();
    $('.error').show();
    $('.error').append($('<label/>').text('Error, please, try again.'));
}

function moveLeft() {
    $('.currentMove').prevUntil('#moves', '.move').first().click();
}

function moveRight() {
    $('.currentMove').nextUntil('#moves', '.move').first().click();
}

function moveStart() {
    $('.currentMove').prevUntil('#moves', '.move').last().click();
}

function moveEnd() {
    $('.currentMove').nextUntil('#moves', '.move').last().click();
}

function flipBoard() {
    board.flip();
}

function setStartPosition() {
    board.start();
}

function clearBoard() {
    board.clear();
}

function changeBoardFen() {
    board.position($('#fen').val());
}

function startNewAnalysis() {
    $('#analysisSettings').show();
    $('#engineTable').empty();
    $('#startNewAnalysisBtn').hide();
    $('#positionNavigation').show();
    $('#analysisNavigation').hide();

    board.destroy();
    initializeBoard();
}