var _data = scr_nikodex_wrapper_extract_http_req_a(["count", "image"])
network_busy = not scr_nikodex_wrapper_check_connection()

switch (content_preparation_phase) {
	
	case obj_prepare_states.downloading_image:
		if _data == undefined
			return
		
		// Download all the content needed for the game.
		
		if (noik_count < scr_nikodex_max_noik and scr_nikodex_wrapper_check_connection()) {
			scr_sprite_manager_sprite_load(_data)
			noik_count++
			obj_prepare_rnoik_download()
		} 
		
		break;
		
	case obj_prepare_states.recieving_niko_count:
		// After retrieving the niko count from the API,
		// Start downloading the Noiks.
		global.noik_count = real(_data)
			
		show_debug_message($"total niko count: {global.noik_count}, getting ready to fetch images...")
		content_preparation_phase = obj_prepare_states.downloading_image
			
		obj_prepare_rnoik_download()
		break;
}