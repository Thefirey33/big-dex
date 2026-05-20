
/**
 * This function reads the entirety of a file.
 * @param {string} The file to read from.
 * @returns {string} The returned contents of the file.
 */
function scr_read_all_lines_from_file(filename){
	var _read_str = ""
	
	var _file = file_text_open_read(filename)
	while (!file_text_eoln(_file)) {
		_read_str += $"{file_text_readln(_file)}\n"	
	}
	
	return _read_str
}