CREATE TABLE MatchPredictionResults (
    Id INT,
    HomeTeam NVARCHAR(MAX),
    AwayTeam NVARCHAR(MAX),
    FirstHalfPrediction NVARCHAR(MAX),
    FirstHalfActualScore NVARCHAR(MAX),
    Prediction NVARCHAR(MAX),
    ActualScore NVARCHAR(MAX),
    MatchDate DATETIME,
    ViewOrder INT
)