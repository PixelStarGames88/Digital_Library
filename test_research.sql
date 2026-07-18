SELECT * FROM "publication"
JOIN "publisher" ON "publisher".publisher_id = "publication".publisher_id
JOIN "publicationauthor" ON "publication".publication_id = "publicationauthor".publication_id
JOIN "author" ON "author".author_id = "publicationauthor".author_id
ORDER BY publication_year;