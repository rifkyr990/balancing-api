-- TASK 1: schema dan constraints (Tabel dasar & batasan nilai)

DROP TABLE IF EXISTS PlanningSlots;
DROP TABLE IF EXISTS Plannings;

CREATE TABLE Plannings (
    PlanningId SERIAL PRIMARY KEY,
    RequestCode VARCHAR(50) NOT NULL UNIQUE,
    CandidateToken VARCHAR(50) NOT NULL,
    Status VARCHAR(20) DEFAULT 'COMPLETED',
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE PlanningSlots (
    Id SERIAL PRIMARY KEY,
    PlanningId INT REFERENCES Plannings(PlanningId) ON DELETE CASCADE,
    SlotOrder INT NOT NULL,
    SlotName VARCHAR(50) NOT NULL,
    OriginalQuantity INT CHECK (OriginalQuantity >= 0),
    BalancedQuantity INT CHECK (BalancedQuantity >= 0),
    IsActive BOOLEAN NOT NULL
);


-- TASK 2: seed data

-- Insert Header
INSERT INTO Plannings (RequestCode, CandidateToken) VALUES 
('REQ-001', 'VEH-RIFKY'), -- Normal case
('REQ-002', 'VEH-RIFKY'), -- Semua 0
('REQ-003', 'VEH-RIFKY'), -- Satu slot aktif
('REQ-004', 'VEH-RIFKY'), -- Nilai awal sama
('REQ-005', 'VEH-RIFKY'), -- Total bersisa
('REQ-006', 'VEH-RIFKY'), -- Nilai besar
('REQ-007', 'VEH-RIFKY'), -- Tie dengan sisa
('REQ-008', 'VEH-RIFKY'), -- Normal case 2
('REQ-009', 'VEH-RIFKY'), -- Anomali 1 (Slot nonaktif ada hasil)
('REQ-010', 'VEH-RIFKY'); -- Anomali 2 (Total awal & hasil beda)

-- Insert Detail
INSERT INTO PlanningSlots (PlanningId, SlotOrder, SlotName, OriginalQuantity, BalancedQuantity, IsActive) VALUES
-- 1. Normal Case
(1, 1, 'Hari 1', 10, 20, TRUE), (1, 2, 'Hari 2', 20, 20, TRUE), (1, 3, 'Hari 3', 30, 20, TRUE),
-- 2. Semua 0
(2, 1, 'Hari 1', 0, 0, FALSE), (2, 2, 'Hari 2', 0, 0, FALSE),
-- 3. Satu Slot Aktif
(3, 1, 'Hari 1', 0, 0, FALSE), (3, 2, 'Hari 2', 15, 15, TRUE),
-- 4. Nilai awal sama
(4, 1, 'Hari 1', 10, 10, TRUE), (4, 2, 'Hari 2', 10, 10, TRUE),
-- 5. Total Bersisa
(5, 1, 'Hari 1', 12, 12, TRUE), (5, 2, 'Hari 2', 8, 11, TRUE), (5, 3, 'Hari 3', 14, 11, TRUE),
-- 6. Nilai Besar
(6, 1, 'Hari 1', 100000, 150000, TRUE), (6, 2, 'Hari 2', 200000, 150000, TRUE),
-- 7. Tie dengan sisa
(7, 1, 'Hari 1', 20, 17, TRUE), (7, 2, 'Hari 2', 20, 17, TRUE), (7, 3, 'Hari 3', 10, 16, TRUE),
-- 8. Normal 2
(8, 1, 'Hari 1', 5, 5, TRUE), (8, 2, 'Hari 2', 5, 5, TRUE),
-- 9. Anomali A: Slot nonaktif tapi BalancedQuantity > 0
(9, 1, 'Hari 1', 0, 10, FALSE), (9, 2, 'Hari 2', 20, 10, TRUE),
-- 10. Anomali B: Total awal (30) beda dengan total hasil (20)
(10, 1, 'Hari 1', 15, 10, TRUE), (10, 2, 'Hari 2', 15, 10, TRUE);



-- TASK 3: total validasi query

SELECT p.PlanningId,SUM(ps.OriginalQuantity) AS TotalAwal,SUM(ps.BalancedQuantity) AS TotalHasil
FROM Plannings p
JOIN PlanningSlots ps ON p.PlanningId = ps.PlanningId
GROUP BY p.PlanningId;


-- TASK 4: HISTORY QUERY

SELECT 
    p.RequestCode,
    p.CreatedAt,
    COUNT(CASE WHEN ps.IsActive = TRUE THEN 1 END) AS JumlahSlotAktif,
    SUM(ps.OriginalQuantity) AS TotalAwal,
    SUM(ps.BalancedQuantity) AS TotalHasil,
    p.Status
FROM Plannings p
JOIN PlanningSlots ps ON p.PlanningId = ps.PlanningId
GROUP BY p.PlanningId, p.RequestCode, p.CreatedAt, p.Status
ORDER BY p.CreatedAt DESC;


-- TASK 5: Cari data bermasalah/tidak valid

-- Anomali 1: Slot nonaktif tapi punya nilai hasil
SELECT p.PlanningId, p.RequestCode, 'Slot non-aktif memiliki nilai > 0' AS AlasanAnomali
FROM Plannings p
JOIN PlanningSlots ps ON p.PlanningId = ps.PlanningId
WHERE ps.IsActive = FALSE AND ps.BalancedQuantity > 0

UNION ALL

-- Anomali 2: Total awal dan total hasil tidak cocok
SELECT p.PlanningId, p.RequestCode, 'Total awal dan total hasil tidak sama' AS AlasanAnomali
FROM Plannings p
JOIN PlanningSlots ps ON p.PlanningId = ps.PlanningId
GROUP BY p.PlanningId, p.RequestCode
HAVING SUM(ps.OriginalQuantity) <> SUM(ps.BalancedQuantity);


-- TASK 6: largest adjustment

SELECT 
    SlotName,
    OriginalQuantity,
    BalancedQuantity,
    ABS(BalancedQuantity - OriginalQuantity) AS Selisih
FROM PlanningSlots
ORDER BY Selisih DESC, SlotOrder ASC
LIMIT 3;


-- TASK 7

-- TASK 8: Versi Rebalance Terbaru

CREATE TABLE RebalanceRuns (
    RunId SERIAL PRIMARY KEY,
    PlanningId INT REFERENCES Plannings(PlanningId),
    RunVersion INT NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Seed Data Versi
INSERT INTO RebalanceRuns (PlanningId, RunVersion) VALUES 
(1, 1), (1, 2), (2, 1);

-- Ambil versi terbesar/terbaru saja
SELECT PlanningId, MAX(RunVersion) AS VersiTerbaru
FROM RebalanceRuns
GROUP BY PlanningId;


-- TASK 9
-- TASK 10