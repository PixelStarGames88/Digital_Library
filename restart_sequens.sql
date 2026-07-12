SELECT setval(pg_get_serial_sequence('publisher', 'publisher_id'), COALESCE((SELECT MAX(publisher_id) FROM "publisher"), 1), false);
SELECT setval(pg_get_serial_sequence('publication', 'publication_id'), COALESCE((SELECT MAX(publication_id) FROM "publication"), 1), false);
SELECT setval(pg_get_serial_sequence('author', 'author_id'), COALESCE((SELECT MAX(author_id) FROM "author"), 1), false);