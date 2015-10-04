var board;
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

function createFenString(positionFen) {
    var moveTurn = $('.moveTurn:checked').val();
    var fen = positionFen + ' ' + moveTurn + ' ';

    var castling = '';
    if ($('#w00').prop('checked'))  {
        castling += 'K';
    }

    if ($('#w000').prop('checked')) {
        castling += 'Q';
    }

    if ($('#b00').prop('checked')) {
        castling += 'k';
    }

    if ($('#b000').prop('checked')) {
        castling += 'q';
    }

    fen += castling == '' ? '-' : castling;

    fen += ' - 0 1';
    return fen;
}

function moveTurnChanged() {
    var fenString = createFenString(board.fen());
    $('#fen').val(fenString);
}

function onPositionChange(oldPos, newPos) {
    var fenString = createFenString(ChessBoard.objToFen(newPos));
    $('#fen').val(fenString);
}

function timer() {
    var data = {
        analysisId: id
    }
    $.get('/Home/GetOutput', data)
        .done(handleSuccessSuccess)
        .done(handleEngineAnalysis)
        .fail(handleServerError);
}

function handleEngineAnalysis(analysisContainer) {
    if (analysisContainer.AnalysisStatus == 1) {
        clearInterval(timerId);
    }

    if (analysisContainer.AnalysisStatus == 2) {
        handleServerError();
    }

    $('#engineOutput').show();
    $('#analysisSettings').hide();

    var headerRow = $('<tr><td>Evaluation</td><td>Line</td></tr>');
    $('#engineTable').empty();
    $('#engineTable').append(headerRow);

    var lines = analysisContainer.PositionAnalysis.Lines;
    for (var j = 0; j < lines.length; j++) {
        var cell = $('<td/>');

        var initialPosition = $('#fen').val();
        var game = new Chess(initialPosition);
        var lineInfo = lines[j];

        var moves = lineInfo.Moves.split(' ');
        
        $('<a class="move" hidden="true" href="#"></a>').appendTo(cell).click(function () { makeMove(initialPosition, this); });
        
        for (var i = 0; i < moves.length; i++) {
            (function (n) {
                var processedMove = processAnalysisMove(game, moves[n]);
                if (n % 2 == 0) {
                    var currentMove;
                    if (n == 0 && $('.moveTurn:checked').val() === 'b') {
                        currentMove = n / 2 + 1 + '... ';
                    } else {
                        currentMove = n / 2 + 1 + '. ';
                    }
                    cell.append($(' <b>' + currentMove + '</b>'));
                }
                $('<a class="move" href="#">' + processedMove.description + ' </a>')
                    .appendTo(cell)
                    .click(function () { makeMove(processedMove.fen, this); });
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

function processAnalysisMove(game, move) {
    var from = move.slice(0, 2);
    var to = move.slice(-2);
    
    var moveInfo = game.move({ from: from, to: to });

    return {
        fen: game.fen(),
        description: moveInfo.san
    }
}

function analyze() {
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
    $('#engineTable').hide();
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