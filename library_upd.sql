ALTER TABLE Publication
DROP COLUMN isbn;

ALTER TABLE Publication
ADD COLUMN keywords VARCHAR(255);

ALTER TABLE Publication
ADD COLUMN annotation TEXT;

ALTER TABLE Publication
ADD COLUMN page_count INT;