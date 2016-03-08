var board;
var id;
var timerId;

$(document).ready(function () {
    $("#depthSpinner").spinner();
    $("#depthSpinner").spinner("value", 1);
    $("#outputLinesSpinner").spinner();
    $("#outputLinesSpinner").spinner("value", 1);
    initializeBoard();
});

function onAnalysisLineSelected(event) {
    var initialPosition = event.data.initialPosition;
    board.position(initialPosition, true);

    var $row = $(event.currentTarget);
    $row.siblings(".active").removeClass("active");
    $row.addClass("active");
    fillMovesAreaWithLineInfo($("#movesArea"), event.data.lineInfo.Moves, initialPosition, "moveNumber", "move");
}

function fillMovesAreaWithLineInfo(container, lineInfoString, initialPosition, moveNumberClass, moveClass) {
    var rawMoves = parseRawLineInfo(lineInfoString);
    var movesInSan = convertRawMovesToSan(rawMoves, initialPosition);

    var isWhiteTurn = $(".moveTurn:checked").val() === "w";
    var movesInOrder = orderMoves(movesInSan, isWhiteTurn, initialPosition);

    container.empty();
    for (var i = 0; i < movesInOrder.length; i++) {
        var move = movesInOrder[i];

        var moveNumber = $("<span/>").addClass(moveNumberClass).text(move.moveNumber);
        container.append(moveNumber);
        container.append(createMoveSpan(move.white, moveClass));

        if (move.black !== undefined) {
            container.append(createMoveSpan(move.black, moveClass));
        }
    }
}

function createMoveSpan(move, spanClass) {
    var $moveSpan = $("<span/>").addClass(spanClass).text(move.san);
    $moveSpan.on("click", move, moveClicked);
    return $moveSpan;
}

function moveClicked(event) {
    var $spanMove = $(event.currentTarget);
    $spanMove.siblings().removeClass("selectedMove");
    $spanMove.addClass("selectedMove");
    makeMove(event.data.fen, this);
}

function orderMoves(movesInSan, isWhiteTurn, initialPosition) {
    var orderedSanMoves = [];
    if (!isWhiteTurn) {
        movesInSan = [createMove("...", initialPosition)].concat(movesInSan);
    }

    for (var i = 0; i < movesInSan.length; i += 2) {
        var moveNumber = i / 2 + 1 + ".";
        var move = { "moveNumber": moveNumber, "white": movesInSan[i], "black": movesInSan[i + 1] };
        orderedSanMoves.push(move);
    }

    return orderedSanMoves;
}

function parseRawLineInfo(lineInfoString) {
    var moves = [];
    var movesStrings = lineInfoString.split(" ");

    for (var i = 0; i < movesStrings.length; i++) {
        var move = movesStrings[i];
        var from = move.slice(0, 2);
        var to = move.slice(-2);

        moves.push({ "from": from, "to": to });
    }

    return moves;
}

function convertRawMovesToSan(rawMoves, initialPosition) {
    var game = new Chess(initialPosition);
    var sanMoves = [];

    console.log("=========================");
    for (var i = 0; i < rawMoves.length; i++) {
        var rawMove = rawMoves[i];
        var moveInfo = game.move({ from: rawMove.from, to: rawMove.to });
        console.log(rawMove);
        sanMoves.push(createMove(moveInfo.san, game.fen()));
    }

    return sanMoves;
}

function createMove(san, fen) {
    return { "san": san, "fen": fen };
}

function makeMove(position, caller) {
    $(".move").removeClass("currentMove");
    $(caller).addClass("currentMove");
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
    $("#fen").val(fenString);
}

function onPositionChange(oldPos, newPos) {
    var fenString = createFenString(ChessBoard.objToFen(newPos));
    $("#fen").val(fenString);
}

function timer() {
    var data = {
        analysisId: id
    }
    $.get("/Home/GetOutput", data)
        .done(handleSuccessSuccess)
        .done(handleEngineAnalysis)
        .fail(handleServerError);
}

function handleEngineAnalysis(analysisContainer) {
    if (analysisContainer.AnalysisStatus === 1) {
        clearInterval(timerId);
    }

    if (analysisContainer.AnalysisStatus === 2) {
        handleServerError();
    }

    $("#engineOutput").show();
    $("#analysisSettings").hide();

    var headerRow = $("<tr class='header'><td>Evaluation</td><td>Line</td></tr>");
    $("#engineTable").empty();
    $("#engineTable").append(headerRow);
    var initialPosition = $("#fen").val();
    var lines = analysisContainer.PositionAnalysis.Lines;

    for (var j = 0; j < lines.length; j++) {
        var cell = $("<td/>");
        var lineInfo = lines[j];
        fillMovesAreaWithLineInfo(cell, lineInfo.Moves, initialPosition, "moveNumberLine", "moveLine");

        var $row = $("<tr/>")
            .append("<td>" + lineInfo.Score / 100 + "</td>")
            .append(cell);

        $row.on("click", { lineInfo: lineInfo, initialPosition: $("#fen").val() }, onAnalysisLineSelected);
        $("#engineTable").append($row);
    }

    var info = analysisContainer.PositionAnalysis;
    var infoRow = "<tr  class='footer'><td colspan='2'>" + info.EngneInfo + ", " + info.AnalysisStatistics.Time / 1000 + " sec, depth " + info.AnalysisStatistics.Depth + ", " + info.AnalysisStatistics.Nodes + " nodes" + "</td></tr>";
    $("#engineTable").append(infoRow);
    $("#analysisNavigation").show();
    $("#startNewAnalysisBtn").show();
    $(".moveLine").off("click");
}

function analyze() {
    $("#positionNavigation").hide();
    
    var data = {
        fen: $("#fen").val(),
        depth: $("#depthSpinner").spinner("value"),
        outputLines: $("#outputLinesSpinner").val(),
        engineId: "1"
    }

    $.get("/Home/StartAnalyze", data)
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

    board = new ChessBoard("board", cfg);
}

function handleSuccessSuccess() {
    $(".error").hide();
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
    board.position($("#fen").val());
}

function startNewAnalysis() {
    $("#analysisSettings").show();
    $("#engineTable").empty();
    $("#startNewAnalysisBtn").hide();
    $("#positionNavigation").show();
    $("#analysisNavigation").hide();
    $("#movesArea").empty();

    board.destroy();
    initializeBoard();
}