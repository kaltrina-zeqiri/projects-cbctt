Select TOP 1 count(id), base_parent_code From courses Where semester_id = 1 AND detail = 'laboratory' Group by base_parent_code ORDER BY count(id) DESC;
 
SElect *  from courses where base_parent_code = 'p001'and detail= 'laboratory'