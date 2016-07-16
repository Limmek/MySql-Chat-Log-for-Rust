<?php 
	include_once'config.php';
	
	$start=$_SESSION['start'];
	$limit=$_SESSION['limit'];
	mysqli_set_charset($dbconfig,"utf8");
	$query=mysqli_query($dbconfig,"select * from mclog ORDER BY TIME DESC LIMIT $start, $limit");
	echo '<ul class="chat">';

	function ip_details($ip)
    {
    	$json = file_get_contents("http://ipinfo.io/{$ip}");
    	$details = json_decode($json);
    	return $details;
    }

	while($result=mysqli_fetch_array($query)) {	
		$ip = explode(':', $result['player_ip'], -1);
		$details = ip_details($ip[0]);	
		if ($result['is_admin']>=1) {
			echo'	<ul class="chat"><li class="right clearfix">
						<span class="chat-img pull-right">
                        	<img src="http://placehold.it/50/FA6F57/fff&amp;text=ADMIN" alt="Admin Avatar" class="img-circle">
                    	</span>
                        <div class="chat-body clearfix">
                            <div class="header">
                                <small class=" text-muted"><img class="img_c" src="http://country.io/flags/48/'.strtolower($details->country).'.png"> <span class="glyphicon glyphicon-time"></span>'.$result['time'].'</small>
                                <strong class="pull-right primary-font">'.$result['player_name'].'</strong>
                            </div>
                            <p>'.$result['chat_msg'].'</p>
                        </div>
                    </li>';

		}else{
	
			echo'<li class="left clearfix">
					<span class="chat-img pull-left">
						<img src="http://placehold.it/50/55C1E7/fff&amp;text=P" alt="Player Avatar" class="img-circle">
					</span>
                	<div class="chat-body clearfix">
                		<div class="header">
                        	<strong class="primary-font">'.$result['player_name'].'</strong> 
                        	<small class="pull-right text-muted"><img class="img_c" src="http://country.io/flags/48/'.strtolower($details->country).'.png"> <span class="glyphicon glyphicon-time"></span> '.$result['time'].'</small>
                        </div>               
                		<p>'.$result['chat_msg'].'.</p>
            		</div>
				</li>';	
		}

	}
	echo '</ul>';
	return true;
?>
