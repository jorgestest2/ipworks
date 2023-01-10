<?php $sendBuffer = TRUE; ob_start(); ?>
<html>
<head>
<title>IPWorks 2022 Demos - Email With Attachments</title>
<link rel="stylesheet" type="text/css" href="stylesheet.css">
<meta name="description" content="IPWorks 2022 Demos - Email With Attachments">
</head>

<body>

<div id="content">
<h1>IPWorks - Demo Pages</h1>
<h2>Email With Attachments</h2>
<p>Demonstrates the use of the FileMailer component.</p>
<a href="default.php">[Other Demos]</a>
<hr/>

<?php
require_once('../include/ipworks_filemailer.php');
require_once('../include/ipworks_const.php');

?>

<?php
  class MyFileMailer extends IPWorks_FileMailer
  {
    function FireSSLServerAuthentication($param) {
			$param['accept'] = true;
			return $param;
		}
  }
   
?>

<form method=POST>
<center>
<table width="50%">

 <tr><td>Demo File: <td>

<select name=file>
<?php
   if ($handle = opendir(realpath('.'))) {
    echo "Directory handle: $handle\n";
    echo "Files:\n";

    while (false !== ($file = readdir($handle))) {
        if (!is_dir($file))
			echo "<option>" . $file . "</option>";
    }
    closedir($handle);
    }
?>
 </select>
 <tr><td>Recipient:   <td><input type=text name=email value="" size=30>
 <tr><td>Mail Server: <td><input type=text name=mailserver size=50>

 <tr><td><td><input type=submit value="  Send File!  ">

</table>
</center>
</form>

<?php
if ($_SERVER['REQUEST_METHOD'] == "POST") {

  echo "Sending file \"" . $_POST["file"] . "\"... ";

  $mailer = new MyFileMailer();
  $mailer->setMailServer($_POST["mailserver"]);
  $mailer->setFrom("IPWorks PHP FileMailer Demo <filemailer@mysite.com>");
  $mailer->setSendTo($_POST["email"]);
  $mailer->setSubject($_POST["file"] . ": the file you requested.");

  $mailer->setMessageText("The requested file, " . $_POST["file"] . ", is attached.");

  $mailer->setAttachmentCount(1);
  $mailer->setAttachmentFile(0,realpath('.') . "\\" . $_POST["file"]);

  try{
    $mailer->doSend();
 	  echo "File Sent!";
  } catch (Exception $e) {
    echo 'Error: ',  $e->getMessage(), "\n";
 }
}
?>

<br/>
<br/>
<br/>
<hr/>
NOTE: These pages are simple demos, and by no means complete applications.  They
are intended to illustrate the usage of the IPWorks objects in a simple,
straightforward way.  What we are hoping to demonstrate is how simple it is to
program with our components.  If you want to know more about them, or if you have
questions, please visit <a href="http://www.nsoftware.com/?demopg-IPPHA" target="_blank">www.nsoftware.com</a> or
contact our technical <a href="http://www.nsoftware.com/support/">support</a>.
<br/>
<br/>
Copyright (c) 2023 /n software inc. - All rights reserved.
<br/>
<br/>
</div>

<div id="footer">
<center>
IPWorks 2022 - Copyright (c) 2023 /n software inc. - All rights reserved. - For more information, please visit our website at <a href="http://www.nsoftware.com/?demopg-IPPHA" target="_blank">www.nsoftware.com</a>.
</center>
</div>

</body>
</html>

<?php if ($sendBuffer) ob_end_flush(); else ob_end_clean(); ?>
