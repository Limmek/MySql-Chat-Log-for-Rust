<?php
	include_once'src/config.php';

	$start=0;


	if(isset($_GET['id'])) {
		$id=$_GET['id'];
		$start=($id-1)*$limit;
		$_SESSION['start']=$start;
		$_SESSION['limit']=$limit;
	}
	else {
		$id=1;
		$_SESSION['start']=$start;
		$_SESSION['limit']=$limit;
	}

	$rows=mysqli_num_rows(mysqli_query($dbconfig,'select * from mclog'));
	$total=ceil($rows/$limit); //


?>
<!DOCTYPE html>
<html lang="en" xmlns="https://www.w3.org/1999/xhtml">
<head>
	<meta charset="utf-8">
	<meta name="viewport" content="width=device-width, initial-scale=1">
	<link rel="stylesheet" href="src/style.css">
  	<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css">
  	<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
  	<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js"></script>
	<script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.5.7/angular.min.js"></script>
	<script type="text/javascript" src="http://code.jquery.com/jquery-1.10.1.min.js"></script>
	<script type="text/javascript">// <![CDATA[
        $(document).ready(function() {
        	$.ajaxSetup({ cache: false }); // This part addresses an IE bug. without it, IE will only load the first number and will never refresh
        	setInterval(function() {
        	$('.tes').load('src/chat.php');
        	}, 3000); // the "3000" here refers to the time to refresh the div. it is in milliseconds.
        });
        // ]]>
    </script>
</head>
<body>
	<div class="container">
		<div class="row row-centered">
			<br><br>
			<div class="col-md-10 col-centered top">
				<div class="panel panel-info">
					<div class="panel-heading"><span class="glyphicon glyphicon-comment"></span> Chat Log
					</div>
					<div class="panel-body">
						<div class="tes"><center><img src="src/processing.gif"></center></div>
					</div>
				  	<div class="panel-footer">
				  		<center>
				  		<ul class="pagination pagination-sm">
							<?php 
								if($id>1) {
									//Go to previous page to show previous 10 items. If its in page 1 then it is inactive
									echo '<li><a href="?id='.($id-1).'" class="button"><span class="glyphicon glyphicon-chevron-left"></span> PREVIOUS</a></li>';
								}
								if($id!=$total)	{
									////Go to previous page to show next 10 items.
									echo '<li><a href="?id='.($id+1).'" class="button">NEXT <span class="glyphicon glyphicon-chevron-right"></span></a></li>';
								}
							?>
						</ul>
						</center>
				  	</div>
				</div>
			</div>
			<div class="col-md-10 col-centered">
				<div class="row row-centered">
					<ul class="pagination pagination-sm">
						<?php
							//show all the page link with page number. When click on these numbers go to particular page. 
							for($i=1;$i<=$total;$i++) {
								if($i==$id) { 
									echo '<li class="active"><a href="?id='.$i.'">'.$i.'</a></li>'; 
								}
								else {
									echo '<li><a href="?id='.$i.'">'.$i.'</a></li>'; 
								}
							}
						?>
					</ul>
				</div>
			</div>

		</div>
	</div>

	<footer>
		<div class="row row-centered">
			MySql web Template. <a href="https://github.com/Limmek/MySql-Chat-Log-for-Rust">Get the plugin for your Rust server Here</a>.
			<p>&copy; Copyright 2016 <a href="https://github.com/Limmek/MySql-Chat-Log-for-Rust">Limmek</a>.</p>
		</div>
	</footer>
</body>
</html>
<?php
	ob_end_flush();
?>