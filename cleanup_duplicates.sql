-- Step 1: Check for duplicate emails
SELECT Email, COUNT(*) as DuplicateCount 
FROM EmailOtps 
GROUP BY Email 
HAVING COUNT(*) > 1;

-- Step 2: Delete duplicate emails, keeping only the oldest record for each email
DELETE e1 
FROM EmailOtps e1
INNER JOIN (
    SELECT Email, MIN(Id) as KeepId
    FROM EmailOtps
    GROUP BY Email
    HAVING COUNT(*) > 1
) e2 ON e1.Email = e2.Email
WHERE e1.Id > e2.KeepId;

-- Step 3: Verify no duplicates remain
SELECT Email, COUNT(*) as Count 
FROM EmailOtps 
GROUP BY Email 
HAVING COUNT(*) > 1;

-- If the above query returns no rows, proceed to apply the migration:
-- dotnet ef database update --project DAJDAJ.DataAccess --startup-project DAJDAJ.Web
